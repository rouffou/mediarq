using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.Caching.Distributed;

namespace Mediarq.Caching;

/// <summary>
/// Pipeline behavior that memoizes responses of <see cref="ICacheableRequest"/> requests in an
/// <see cref="IDistributedCache"/> (e.g. Redis), serializing the response via
/// <see cref="IMediarqCacheSerializer"/>. Inert for request types that are not cacheable.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class DistributedCachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly bool RequestIsCacheable = typeof(ICacheableRequest).IsAssignableFrom(typeof(TRequest));

    private readonly IDistributedCache _cache;
    private readonly IMediarqCacheSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedCachingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache used to store responses.</param>
    /// <param name="serializer">The serializer used to convert responses to and from bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cache"/> or <paramref name="serializer"/> is <see langword="null"/>.</exception>
    public DistributedCachingBehavior(IDistributedCache cache, IMediarqCacheSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(serializer);
        _cache = cache;
        _serializer = serializer;
    }

    /// <summary>Active only for request types that implement <see cref="ICacheableRequest"/>.</summary>
    public bool IsActive => RequestIsCacheable;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var cacheable = (ICacheableRequest)context.Request!;
        var key = cacheable.CacheKey;

        var cachedBytes = await _cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (cachedBytes is { Length: > 0 })
        {
            var cached = _serializer.Deserialize<TResponse>(cachedBytes);
            if (cached is not null)
            {
                return cached;
            }
        }

        var response = await handle().ConfigureAwait(false);

        // Only cache a non-null response; a null cannot be round-tripped meaningfully.
        if (response is not null)
        {
            var options = new DistributedCacheEntryOptions();
            if (cacheable.CacheDuration is { } duration)
            {
                options.AbsoluteExpirationRelativeToNow = duration;
            }

            await _cache.SetAsync(key, _serializer.Serialize(response), options, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
