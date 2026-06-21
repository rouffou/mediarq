using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    /// <summary>
    /// Registers the <see cref="DistributedCachingBehavior{TRequest, TResponse}"/> so responses of
    /// <see cref="ICacheableRequest"/> requests are memoized in an <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>
    /// (e.g. Redis), plus a default JSON <see cref="IMediarqCacheSerializer"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>. You must register an
    /// <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/> separately (for example
    /// <c>AddStackExchangeRedisCache</c>, or <c>AddDistributedMemoryCache</c> for a single process).
    /// Register your own <see cref="IMediarqCacheSerializer"/> before this call to override the default
    /// JSON serialization (also the way to stay reflection-free on Native AOT).
    /// </remarks>
    public static IServiceCollection AddMediarqDistributedCaching(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IMediarqCacheSerializer, JsonMediarqCacheSerializer>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DistributedCachingBehavior<,>));
        return services;
    }
}
