using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.RateLimiting;

/// <summary>
/// Extension methods that register the Mediarq rate limiting behavior.
/// </summary>
public static class RateLimitingServiceCollectionExtensions
{
    /// <summary>
    /// Registers a <see cref="RateLimiterRegistry"/> configured by <paramref name="configure"/> and the
    /// <see cref="RateLimitingBehavior{TRequest, TResponse}"/> so <see cref="IRateLimitedRequest"/>
    /// requests acquire a permit from their named policy before their handler runs.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Registers the named policies via <see cref="RateLimiterRegistry.AddPolicy"/>.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddMediarqRateLimiting(this IServiceCollection services, Action<RateLimiterRegistry> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var registry = new RateLimiterRegistry();
        configure(registry);

        services.AddSingleton(registry);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RateLimitingBehavior<,>));
        return services;
    }
}
