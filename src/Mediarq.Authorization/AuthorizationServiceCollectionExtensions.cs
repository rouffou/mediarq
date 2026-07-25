using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Authorization;

/// <summary>
/// Extension methods that register the Mediarq authorization behavior.
/// </summary>
public static class AuthorizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="AuthorizationBehavior{TRequest, TResponse}"/> so <see cref="IAuthorizedRequest"/>
    /// requests are authorized before their handler runs.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Requires <c>services.AddAuthorization(...)</c> and <c>services.AddHttpContextAccessor()</c> to
    /// already be registered. Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>.
    /// </remarks>
    public static IServiceCollection AddMediarqAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        return services;
    }
}
