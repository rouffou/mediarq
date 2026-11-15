namespace Mediarq.Caching;

/// <summary>
/// Marks a request whose response can be cached. The <see cref="CachingBehavior{TRequest, TResponse}"/>
/// returns the cached response on a hit, and stores the handler's response on a miss.
/// </summary>
public interface ICacheableRequest
{
    /// <summary>Gets the cache key uniquely identifying this request instance.</summary>
    string CacheKey { get; }

    /// <summary>
    /// Gets how long the response stays cached (sliding from insertion). When <see langword="null"/>,
    /// no relative expiration is set and the entry follows the cache's own policy.
    /// </summary>
    TimeSpan? CacheDuration => null;

    /// <summary>
    /// Gets whether a failed <see cref="Mediarq.Core.Common.Results.Result"/> is cached too. Defaults to
    /// <see langword="false"/>: a failure is usually transient and must not be replayed from the cache.
    /// </summary>
    bool CacheFailures => false;
}
