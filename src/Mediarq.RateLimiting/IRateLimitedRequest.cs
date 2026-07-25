namespace Mediarq.RateLimiting;

/// <summary>
/// Marks a request that must acquire a permit from a named rate limit policy before its handler runs.
/// </summary>
public interface IRateLimitedRequest
{
    /// <summary>
    /// The name of the policy to acquire a permit from, registered via
    /// <c>services.AddMediarqRateLimiting(registry =&gt; registry.AddPolicy(name, ...))</c>.
    /// </summary>
    string PolicyName { get; }

    /// <summary>
    /// The partition to acquire the permit within (e.g. the current user id), so different callers get
    /// independent limits under the same policy. <see langword="null"/> partitions every request under
    /// the policy into a single shared bucket.
    /// </summary>
    string? PartitionKey => null;
}
