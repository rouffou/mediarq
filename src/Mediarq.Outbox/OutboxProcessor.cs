using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mediarq.Outbox;

/// <summary>
/// Background service that periodically publishes pending <see cref="OutboxMessage"/> rows from
/// <typeparamref name="TContext"/> through the Mediarq <see cref="IPublisher"/>, providing at-least-once
/// delivery of enqueued notifications.
/// </summary>
/// <typeparam name="TContext">The EF Core <see cref="DbContext"/> that maps <see cref="OutboxMessage"/>.</typeparam>
public sealed class OutboxProcessor<TContext> : BackgroundService
    where TContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxProcessor<TContext>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxProcessor{TContext}"/> class.
    /// </summary>
    /// <param name="scopeFactory">Factory used to create a scope per poll (for a scoped context/publisher).</param>
    /// <param name="options">The processor options (polling interval, batch size).</param>
    /// <param name="logger">Optional logger for processing errors.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scopeFactory"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public OutboxProcessor(IServiceScopeFactory scopeFactory, OutboxOptions options, ILogger<OutboxProcessor<TContext>>? logger = null)
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
        using var timer = new PeriodicTimer(_options.PollingInterval);

        do
        {
            try
            {
                await ProcessOnceAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Mediarq outbox processing failed; will retry on the next poll.");
            }
        }
        while (await WaitForNextTickAsync(timer, stoppingToken).ConfigureAwait(false));
    }

    private async Task ProcessOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        await OutboxDispatcher.ProcessPendingAsync(context, publisher, _options.BatchSize, cancellationToken).ConfigureAwait(false);
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
