using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mediarq.Idempotency.EntityFrameworkCore;

/// <summary>
/// Background service that periodically removes expired <see cref="IdempotencyCacheEntry"/> rows from
/// <typeparamref name="TContext"/>. Expired entries are already ignored on read
/// (<see cref="EfCoreDistributedCache{TContext}.GetAsync"/>); this only reclaims storage for keys that
/// are never read again after expiring.
/// </summary>
/// <typeparam name="TContext">The EF Core <see cref="DbContext"/> that maps <see cref="IdempotencyCacheEntry"/>.</typeparam>
public sealed class IdempotencyCacheCleanupService<TContext> : BackgroundService
    where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IdempotencyCacheOptions _options;
    private readonly ILogger<IdempotencyCacheCleanupService<TContext>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyCacheCleanupService{TContext}"/> class.
    /// </summary>
    /// <param name="scopeFactory">Factory used to create a scope per sweep (for a scoped context).</param>
    /// <param name="options">The cleanup options (interval, batch size).</param>
    /// <param name="logger">Optional logger for cleanup errors.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scopeFactory"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public IdempotencyCacheCleanupService(IServiceScopeFactory scopeFactory, IdempotencyCacheOptions options, ILogger<IdempotencyCacheCleanupService<TContext>>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(options);
        _scopeFactory = scopeFactory;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_options.CleanupInterval);

        do
        {
            try
            {
                await CleanupOnceAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Mediarq idempotency cache cleanup failed; will retry on the next sweep.");
            }
        }
        while (await WaitForNextTickAsync(timer, stoppingToken).ConfigureAwait(false));
    }

    private async Task CleanupOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        var now = DateTimeOffset.UtcNow;
        var expired = await context.Set<IdempotencyCacheEntry>()
            .Where(e => e.ExpiresAtUtc != null && e.ExpiresAtUtc <= now)
            .Take(_options.CleanupBatchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (expired.Count == 0)
        {
            return;
        }

        context.Set<IdempotencyCacheEntry>().RemoveRange(expired);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task<bool> WaitForNextTickAsync(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        try
        {
            return await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}
