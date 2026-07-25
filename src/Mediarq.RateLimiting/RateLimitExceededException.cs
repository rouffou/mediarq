namespace Mediarq.RateLimiting;

/// <summary>
/// Thrown when a request could not acquire a permit from its rate limit policy. Catch this — e.g. via an
/// <c>IRequestExceptionHandler&lt;TRequest, TResponse&gt;</c> or an ASP.NET Core exception handler — to
/// map it to a domain failure or an HTTP <c>429 Too Many Requests</c> response.
/// </summary>
/// <param name="policyName">The rate limit policy that rejected the request.</param>
/// <param name="partitionKey">The partition the request was rejected under.</param>
/// <param name="retryAfter">How long to wait before retrying, when the limiter reports one.</param>
public sealed class RateLimitExceededException(string policyName, string partitionKey, TimeSpan? retryAfter = null)
    : Exception($"Rate limit policy '{policyName}' rejected the request for partition '{partitionKey}'.")
{
    /// <summary>The rate limit policy that rejected the request.</summary>
    public string PolicyName { get; } = policyName;

    /// <summary>The partition the request was rejected under.</summary>
    public string PartitionKey { get; } = partitionKey;

    /// <summary>How long to wait before retrying, when the limiter reports one.</summary>
    public TimeSpan? RetryAfter { get; } = retryAfter;
}
