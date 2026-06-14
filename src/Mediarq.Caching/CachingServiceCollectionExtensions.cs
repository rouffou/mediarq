using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Caching;

/// <summary>
/// Extension methods that register the Mediarq caching behavior.
/// </summary>
public static class CachingServiceCollectionExtensions
{
    /// <summary>
    /// Registers an in-memory cache and the <see cref="CachingBehavior{TRequest, TResponse}"/> so
    /// responses of <see cref="ICacheableRequest"/> requests are memoized.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>. Uses <c>AddMemoryCache()</c> if no
    /// <c>IMemoryCache</c> is registered yet.</remarks>
    public static IServiceCollection AddMediarqCaching(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMemoryCache();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        return services;
    }
}
