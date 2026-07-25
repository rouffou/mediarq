using System.Reflection;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Mediarq.AspNetCore;

/// <summary>
/// Maps commands/queries annotated with a Mediarq route attribute (<see cref="MediarqGetAttribute"/>,
/// <see cref="MediarqPostAttribute"/>, <see cref="MediarqPutAttribute"/>, <see cref="MediarqPatchAttribute"/>,
/// <see cref="MediarqDeleteAttribute"/>) directly as minimal API endpoints, converting their <see cref="Result"/>/
/// <see cref="Result{T}"/> response the same way <see cref="ResultHttpExtensions.ToHttpResult(Result)"/> does. A
/// no-result <see cref="Core.Common.Requests.Command.ICommand"/> (response type <see cref="Unit"/>) maps a
/// successful dispatch to <c>204 No Content</c>.
/// </summary>
public static class MediarqEndpointRouteBuilderExtensions
{
    private static readonly MethodInfo HandleBodyMethod =
        typeof(MediarqEndpointRouteBuilderExtensions).GetMethod(nameof(HandleBody), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo HandleBodyWithValueMethod =
        typeof(MediarqEndpointRouteBuilderExtensions).GetMethod(nameof(HandleBodyWithValue), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo HandleParamsMethod =
        typeof(MediarqEndpointRouteBuilderExtensions).GetMethod(nameof(HandleParams), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo HandleParamsWithValueMethod =
        typeof(MediarqEndpointRouteBuilderExtensions).GetMethod(nameof(HandleParamsWithValue), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo HandleUnitBodyMethod =
        typeof(MediarqEndpointRouteBuilderExtensions).GetMethod(nameof(HandleUnitBody), BindingFlags.NonPublic | BindingFlags.Static)!;
    private static readonly MethodInfo HandleUnitParamsMethod =
        typeof(MediarqEndpointRouteBuilderExtensions).GetMethod(nameof(HandleUnitParams), BindingFlags.NonPublic | BindingFlags.Static)!;

    /// <summary>
    /// Scans <paramref name="assemblies"/> (the entry assembly, when none are supplied) for types carrying
    /// a Mediarq route attribute and maps each as a minimal API endpoint, dispatching through <see cref="ISender"/>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder, e.g. a <c>WebApplication</c>.</param>
    /// <param name="assemblies">Assemblies to scan; defaults to the entry assembly when none are supplied.</param>
    /// <returns>
    /// A <see cref="RouteGroupBuilder"/> containing every mapped endpoint, so shared conventions
    /// (<c>.RequireAuthorization()</c>, <c>.WithTags(...)</c>, ...) can be applied to all of them at once.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="endpoints"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown at startup if an attributed type does not implement <see cref="ICommandOrQuery{TResponse}"/>, or its
    /// response type is none of <see cref="Result"/>, <see cref="Result{T}"/> or <see cref="Unit"/> — the only
    /// shapes this can convert to HTTP.
    /// </exception>
    /// <remarks>
    /// <see cref="MediarqGetAttribute"/>/<see cref="MediarqDeleteAttribute"/> bind the request's members
    /// individually from the route/query string (<c>[AsParameters]</c>) — there is no request body on a GET/DELETE.
    /// <see cref="MediarqPostAttribute"/>/<see cref="MediarqPutAttribute"/>/<see cref="MediarqPatchAttribute"/>
    /// bind the whole request from the JSON body.
    /// </remarks>
    public static RouteGroupBuilder MapMediarq(this IEndpointRouteBuilder endpoints, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var group = endpoints.MapGroup(string.Empty);
        var scanTargets = assemblies is { Length: > 0 }
            ? assemblies
            : [Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()];

        foreach (var assembly in scanTargets.Distinct())
        {
            foreach (var type in assembly.GetTypes())
            {
                var route = GetRoute(type);
                if (route is null)
                {
                    continue;
                }

                MapEndpoint(group, type, route.Value.Method, route.Value.Pattern);
            }
        }

        return group;
    }

    private static (string Method, string Pattern)? GetRoute(Type type)
    {
        if (type.GetCustomAttribute<MediarqGetAttribute>() is { } get)
        {
            return ("GET", get.Pattern);
        }

        if (type.GetCustomAttribute<MediarqPostAttribute>() is { } post)
        {
            return ("POST", post.Pattern);
        }

        if (type.GetCustomAttribute<MediarqPutAttribute>() is { } put)
        {
            return ("PUT", put.Pattern);
        }

        if (type.GetCustomAttribute<MediarqPatchAttribute>() is { } patch)
        {
            return ("PATCH", patch.Pattern);
        }

        if (type.GetCustomAttribute<MediarqDeleteAttribute>() is { } delete)
        {
            return ("DELETE", delete.Pattern);
        }

        return null;
    }

    private static void MapEndpoint(RouteGroupBuilder group, Type requestType, string method, string pattern)
    {
        var responseType = GetResponseType(requestType)
            ?? throw new InvalidOperationException(
                $"'{requestType}' has a Mediarq route attribute but does not implement ICommandOrQuery<TResponse>.");

        var bindFromParameters = method is "GET" or "DELETE";
        Delegate handler;

        if (responseType == typeof(Result))
        {
            var closed = (bindFromParameters ? HandleParamsMethod : HandleBodyMethod).MakeGenericMethod(requestType);
            handler = closed.CreateDelegate(HandlerDelegateType(requestType));
        }
        else if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var closed = (bindFromParameters ? HandleParamsWithValueMethod : HandleBodyWithValueMethod).MakeGenericMethod(requestType, valueType);
            handler = closed.CreateDelegate(HandlerDelegateType(requestType));
        }
        else if (responseType == typeof(Unit))
        {
            var closed = (bindFromParameters ? HandleUnitParamsMethod : HandleUnitBodyMethod).MakeGenericMethod(requestType);
            handler = closed.CreateDelegate(HandlerDelegateType(requestType));
        }
        else
        {
            throw new InvalidOperationException(
                $"'{requestType}' has a Mediarq route attribute but its response type '{responseType}' is not Result, Result<T> or Unit (a no-result ICommand).");
        }

        _ = method switch
        {
            "GET" => group.MapGet(pattern, handler),
            "POST" => group.MapPost(pattern, handler),
            "PUT" => group.MapPut(pattern, handler),
            "PATCH" => group.MapPatch(pattern, handler),
            "DELETE" => group.MapDelete(pattern, handler),
            _ => throw new InvalidOperationException($"Unsupported HTTP method '{method}'."),
        };
    }

    private static Type HandlerDelegateType(Type requestType) =>
        typeof(Func<,,,>).MakeGenericType(requestType, typeof(ISender), typeof(CancellationToken), typeof(Task<IResult>));

    private static Type? GetResponseType(Type requestType) =>
        requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandOrQuery<>))
            ?.GetGenericArguments()[0];

    private static async Task<IResult> HandleBody<TRequest>(TRequest request, ISender sender, CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<Result>
        => (await sender.Send<Result>(request, cancellationToken).ConfigureAwait(false)).ToHttpResult();

    private static async Task<IResult> HandleBodyWithValue<TRequest, TValue>(TRequest request, ISender sender, CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<Result<TValue>>
        => (await sender.Send<Result<TValue>>(request, cancellationToken).ConfigureAwait(false)).ToHttpResult();

    private static async Task<IResult> HandleParams<TRequest>([AsParameters] TRequest request, ISender sender, CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<Result>
        => (await sender.Send<Result>(request, cancellationToken).ConfigureAwait(false)).ToHttpResult();

    private static async Task<IResult> HandleParamsWithValue<TRequest, TValue>([AsParameters] TRequest request, ISender sender, CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<Result<TValue>>
        => (await sender.Send<Result<TValue>>(request, cancellationToken).ConfigureAwait(false)).ToHttpResult();

    // No-result ICommand (response type Unit): there is no Result to unwrap, so success maps directly to
    // 204 No Content -- a handler exception propagates as an unhandled exception (500), same as it would
    // for any other minimal API endpoint.
    private static async Task<IResult> HandleUnitBody<TRequest>(TRequest request, ISender sender, CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<Unit>
    {
        await sender.Send<Unit>(request, cancellationToken).ConfigureAwait(false);
        return TypedResults.NoContent();
    }

    private static async Task<IResult> HandleUnitParams<TRequest>([AsParameters] TRequest request, ISender sender, CancellationToken cancellationToken)
        where TRequest : ICommandOrQuery<Unit>
    {
        await sender.Send<Unit>(request, cancellationToken).ConfigureAwait(false);
        return TypedResults.NoContent();
    }
}
