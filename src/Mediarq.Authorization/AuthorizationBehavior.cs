using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Mediarq.Authorization;

/// <summary>
/// Pipeline behavior that runs ASP.NET Core authorization for <see cref="IAuthorizedRequest"/> requests
/// before the handler: no authenticated user short-circuits with <see cref="ResultError.Unauthorized"/>
/// (401), a failed policy short-circuits with <see cref="ResultError.Forbidden"/> (403). Inert for
/// request types that are not <see cref="IAuthorizedRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request. Must be <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private const string DynamicFallbackJustification =
        "The dynamic Result<T> failure factory is only built when TResponse is a closed Result<T> not " +
        "equal to the reflection-free Result case, which requires MakeGenericMethod.";

    private static readonly bool RequestIsAuthorized = typeof(IAuthorizedRequest).IsAssignableFrom(typeof(TRequest));

    // Reflection-free path for TResponse == Result; null for everything else, including Result<T>.
    private static readonly Func<ResultError, TResponse>? FailureFactory = BuildSafeFailureFactory();

    // Lazily-built, cached reflection fallback for Result<T> responses.
    private static Func<ResultError, TResponse>? _dynamicFailureFactory;

    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="authorizationService">Evaluates the request's <see cref="IAuthorizedRequest.PolicyName"/>.</param>
    /// <param name="httpContextAccessor">Provides the current user's <see cref="System.Security.Claims.ClaimsPrincipal"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if either parameter is <see langword="null"/>.</exception>
    public AuthorizationBehavior(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(authorizationService);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>Active only for request types that implement <see cref="IAuthorizedRequest"/>.</summary>
    public bool IsActive => RequestIsAuthorized;

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = DynamicFallbackJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = DynamicFallbackJustification)]
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var authorized = (IAuthorizedRequest)context.Request!;
        var principal = _httpContextAccessor.HttpContext?.User;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return BuildFailure(ResultError.Unauthorized("Authorization.Unauthenticated", "Authentication is required to perform this operation."));
        }

        if (authorized.PolicyName is not null)
        {
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(principal, context.Request, authorized.PolicyName)
                .ConfigureAwait(false);

            if (!authorizationResult.Succeeded)
            {
                return BuildFailure(ResultError.Forbidden("Authorization.Forbidden", $"You do not have permission to perform this operation (policy '{authorized.PolicyName}')."));
            }
        }

        return await handle().ConfigureAwait(false);
    }

    private static TResponse BuildFailure(ResultError error)
    {
        var factory = FailureFactory ?? ResolveDynamicFailureFactory();

        if (factory is null)
        {
            throw new InvalidOperationException($"Authorization failed but TResponse type '{typeof(TResponse).Name}' is not a supported Result type.");
        }

        return factory(error);
    }

    private static Func<ResultError, TResponse>? BuildSafeFailureFactory()
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return static error => (TResponse)(object)Result.Failure(error);
        }

        return null;
    }

    [RequiresUnreferencedCode(DynamicFallbackJustification)]
    [RequiresDynamicCode(DynamicFallbackJustification)]
    private static Func<ResultError, TResponse>? ResolveDynamicFailureFactory()
    {
        if (_dynamicFailureFactory is not null)
        {
            return _dynamicFailureFactory;
        }

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];
            MethodInfo method = typeof(Result)
                .GetMethods()
                .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition)
                .MakeGenericMethod(valueType);

            ParameterExpression errorParam = Expression.Parameter(typeof(ResultError), "error");
            Expression body = Expression.Convert(Expression.Call(method, errorParam), typeof(TResponse));

            _dynamicFailureFactory = Expression.Lambda<Func<ResultError, TResponse>>(body, errorParam).Compile();
            return _dynamicFailureFactory;
        }

        return null;
    }
}
