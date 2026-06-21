using Mediarq.Caching;
using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mediarq.Idempotency;

/// <summary>
/// Extension methods that register the Mediarq idempotency behavior.
/// </summary>
public static class IdempotencyServiceCollectionExtensions
{
    /// <summary>
    /// Registers the <see cref="IdempotencyBehavior{TRequest, TResponse}"/> so requests implementing
    /// <see cref="IIdempotentRequest"/> run at most once per key, plus a default JSON
    /// <see cref="IMediarqCacheSerializer"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same service collection, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Call after <c>AddMediarq</c>/<c>AddMediarqCore</c>. You must register an
    /// <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/> separately (for example
    /// <c>AddStackExchangeRedisCache</c>, or <c>AddDistributedMemoryCache</c> for a single process).
    /// Register your own <see cref="IMediarqCacheSerializer"/> beforehand to override the default JSON
    /// serialization (also the way to stay reflection-free on Native AOT).
    /// </remarks>
    public static IServiceCollection AddMediarqIdempotency(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IMediarqCacheSerializer, JsonMediarqCacheSerializer>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        return services;
    }
}
