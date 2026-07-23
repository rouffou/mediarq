using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Idempotency.EntityFrameworkCore;

/// <summary>
/// Extension methods that wire an EF Core-backed <see cref="IDistributedCache"/> for Mediarq.Idempotency.
/// </summary>
public static class IdempotencyCacheServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="EfCoreDistributedCache{TContext}"/> as the <see cref="IDistributedCache"/>
    /// used by <c>Mediarq.Idempotency</c>, plus a background service that periodically removes expired
    /// entries.
    /// </summary>
    /// <typeparam name="TContext">
    /// The EF Core <see cref="DbContext"/> that maps <see cref="IdempotencyCacheEntry"/>
    /// (see <see cref="IdempotencyCacheModelBuilderExtensions.ApplyMediarqIdempotencyCache"/>).
    /// </typeparam>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">Optional callback to tune the cleanup service (interval, batch size).</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call instead of <c>AddDistributedMemoryCache()</c> / <c>AddStackExchangeRedisCache()</c>, before
    /// <c>AddMediarqIdempotency()</c>. Map the cache table by calling
    /// <c>ApplyMediarqIdempotencyCache()</c> in your context's <c>OnModelCreating</c>.
    /// </remarks>
    public static IServiceCollection AddMediarqIdempotencyEntityFrameworkCore<TContext>(this IServiceCollection services, Action<IdempotencyCacheOptions>? configure = null)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new IdempotencyCacheOptions();
        configure?.Invoke(options);

        services.TryAddSingleton(options);
        // Scoped, not singleton: this implementation is tied to the scoped TContext instance.
        services.AddScoped<IDistributedCache, EfCoreDistributedCache<TContext>>();
        services.AddHostedService<IdempotencyCacheCleanupService<TContext>>();

        return services;
    }
}
