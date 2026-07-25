using System.Reflection;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.HealthChecks;

/// <summary>
/// Reports on a command/query type whose closed <c>IRequestHandler&lt;TRequest, TResponse&gt;</c>
/// registration count is not exactly one.
/// </summary>
/// <param name="RequestType">The concrete <see cref="ICommandOrQuery{TResponse}"/> type that was checked.</param>
/// <param name="HandlerCount">How many handlers are registered for it (0 = missing, 2+ = ambiguous).</param>
public sealed record HandlerRegistrationIssue(Type RequestType, int HandlerCount)
{
    /// <inheritdoc />
    public override string ToString() => HandlerCount == 0
        ? $"No handler is registered for '{RequestType}'."
        : $"{HandlerCount} handlers are registered for '{RequestType}'; expected exactly 1.";
}

/// <summary>
/// Scans a set of assemblies for closed <see cref="ICommandOrQuery{TResponse}"/> types and verifies,
/// against a built <see cref="IServiceProvider"/>, that each one resolves to exactly one
/// <c>IRequestHandler&lt;TRequest, TResponse&gt;</c>.
/// </summary>
public static class MediarqHandlerRegistrationValidator
{
    /// <summary>
    /// Validates handler registrations. Requests are discovered by scanning <paramref name="assemblies"/>
    /// (or the entry assembly, when none are supplied) for non-abstract classes/records implementing
    /// <see cref="ICommandOrQuery{TResponse}"/>.
    /// </summary>
    /// <returns>One <see cref="HandlerRegistrationIssue"/> per request type that has zero or more than one registered handler.</returns>
    public static IReadOnlyList<HandlerRegistrationIssue> Validate(IServiceProvider serviceProvider, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        var scanTargets = assemblies is { Length: > 0 }
            ? assemblies
            : [Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()];

        List<HandlerRegistrationIssue>? issues = null;

        foreach (var requestType in DiscoverRequestTypes(scanTargets))
        {
            var responseType = GetResponseType(requestType);
            if (responseType is null)
            {
                continue;
            }

            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            var count = serviceProvider.GetServices(handlerType).Count();

            if (count != 1)
            {
                (issues ??= []).Add(new HandlerRegistrationIssue(requestType, count));
            }
        }

        return issues ?? (IReadOnlyList<HandlerRegistrationIssue>)[];
    }

    private static IEnumerable<Type> DiscoverRequestTypes(IReadOnlyList<Assembly> assemblies)
    {
        var seen = new HashSet<Type>();

        foreach (var assembly in assemblies.Distinct())
        {
            Type?[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }

            foreach (var type in types)
            {
                if (type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
                    && typeof(ICommandOrQuery<>) is var marker
                    && type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == marker)
                    && seen.Add(type))
                {
                    yield return type;
                }
            }
        }
    }

    private static Type? GetResponseType(Type requestType) =>
        requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandOrQuery<>))
            ?.GetGenericArguments()[0];
}
