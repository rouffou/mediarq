namespace Mediarq.Idempotency.EntityFrameworkCore;

/// <summary>Options controlling the <see cref="IdempotencyCacheCleanupService{TContext}"/>.</summary>
public sealed class IdempotencyCacheOptions
{
    /// <summary>Gets or sets how often the cleanup service sweeps for expired entries. Default: 5 minutes.</summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>Gets or sets the maximum number of expired entries removed per sweep. Default: 200.</summary>
    public int CleanupBatchSize { get; set; } = 200;
}
