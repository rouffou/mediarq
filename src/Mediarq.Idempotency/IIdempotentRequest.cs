namespace Mediarq.Idempotency;

/// <summary>
/// Marks a request that must run at most once per <see cref="IdempotencyKey"/>. When the
/// <c>IdempotencyBehavior</c> is registered (via <c>AddMediarqIdempotency()</c>), a repeated request
/// with the same key returns the previously stored result instead of re-running the handler.
/// </summary>
/// <remarks>
/// Use a stable, caller-supplied key (for example an <c>Idempotency-Key</c> HTTP header or a
/// business identifier). The behavior performs a best-effort check-then-store against an
/// <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>; for strict
/// once-only semantics under high concurrency, pair it with a distributed lock or a store that supports
/// atomic set-if-absent.
/// </remarks>
public interface IIdempotentRequest
{
    /// <summary>Gets the key that uniquely identifies this logical operation.</summary>
    string IdempotencyKey { get; }

    /// <summary>
    /// Gets how long the result is retained for replay. When <see langword="null"/>, a default retention
    /// is used.
    /// </summary>
    TimeSpan? IdempotencyDuration => null;
}
