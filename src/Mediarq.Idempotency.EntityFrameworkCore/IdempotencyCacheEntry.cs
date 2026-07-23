namespace Mediarq.Idempotency.EntityFrameworkCore;

/// <summary>
/// A persisted cache entry backing <see cref="EfCoreDistributedCache{TContext}"/>.
/// </summary>
public sealed class IdempotencyCacheEntry
{
    /// <summary>Gets or sets the cache key.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Gets or sets the cached value.</summary>
    public byte[] Value { get; set; } = [];

    /// <summary>Gets or sets the hard expiration ceiling, if any (<c>AbsoluteExpiration</c>/<c>AbsoluteExpirationRelativeToNow</c>).</summary>
    public DateTimeOffset? AbsoluteExpiration { get; set; }

    /// <summary>Gets or sets the sliding expiration window, if any; extends <see cref="ExpiresAtUtc"/> on each read, capped by <see cref="AbsoluteExpiration"/>.</summary>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>Gets or sets when this entry currently expires; <see langword="null"/> means it never expires.</summary>
    public DateTimeOffset? ExpiresAtUtc { get; set; }
}
