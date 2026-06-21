using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.Caching.Memory;

namespace Mediarq.Caching;

/// <summary>
/// Pipeline behavior that memoizes responses of requests implementing <see cref="ICacheableRequest"/>.
/// Requests that do not implement it pass through untouched.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly bool RequestIsCacheable = typeof(ICacheableRequest).IsAssignableFrom(typeof(TRequest));

    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The memory cache used to store responses.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cache"/> is <see langword="null"/>.</exception>
    public CachingBehavior(IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(cache);
        _cache = cache;
    }

    /// <summary>Active only for request types that implement <see cref="ICacheableRequest"/>.</summary>
    public bool IsActive => RequestIsCacheable;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var cacheable = (ICacheableRequest)context.Request!;

        if (_cache.TryGetValue(cacheable.CacheKey, out TResponse? cached) && cached is not null)
        {
            return cached;
        }

        var response = await handle().ConfigureAwait(false);

        var options = new MemoryCacheEntryOptions();
        if (cacheable.CacheDuration is { } duration)
        {
            options.AbsoluteExpirationRelativeToNow = duration;
        }

        _cache.Set(cacheable.CacheKey, response, options);
        return response;
    }
}
