using System.Collections.Concurrent;
using System.Threading.RateLimiting;

namespace Mediarq.RateLimiting;

/// <summary>
/// A named registry of <see cref="PartitionedRateLimiter{TKey}"/> instances, keyed by policy name.
/// Configure it via <see cref="RateLimitingServiceCollectionExtensions.AddMediarqRateLimiting"/>.
/// </summary>
public sealed class RateLimiterRegistry
{
    private readonly ConcurrentDictionary<string, PartitionedRateLimiter<string>> _limiters = new();

    /// <summary>
    /// Registers a rate limit policy: <paramref name="partitioner"/> maps a partition key (<see cref="IRateLimitedRequest.PartitionKey"/>,
    /// or <c>"*"</c> when <see langword="null"/>) to the <see cref="RateLimitPartition{TKey}"/> that governs it.
    /// </summary>
    /// <param name="name">The policy name, referenced from <see cref="IRateLimitedRequest.PolicyName"/>.</param>
    /// <param name="partitioner">Builds the partition (and its limiter) for a given key.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is null, empty, or already registered.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="partitioner"/> is <see langword="null"/>.</exception>
    public void AddPolicy(string name, Func<string, RateLimitPartition<string>> partitioner)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(partitioner);

        if (!_limiters.TryAdd(name, PartitionedRateLimiter.Create(partitioner)))
        {
            throw new ArgumentException($"A rate limit policy named '{name}' is already registered.", nameof(name));
        }
    }

    /// <summary>Gets the limiter registered for <paramref name="name"/>.</summary>
    /// <exception cref="InvalidOperationException">Thrown when no policy named <paramref name="name"/> is registered.</exception>
    internal PartitionedRateLimiter<string> GetPolicy(string name) =>
        _limiters.TryGetValue(name, out var limiter)
            ? limiter
            : throw new InvalidOperationException($"No rate limit policy named '{name}' is registered. Register it via services.AddMediarqRateLimiting(registry => registry.AddPolicy(\"{name}\", ...)).");
}
