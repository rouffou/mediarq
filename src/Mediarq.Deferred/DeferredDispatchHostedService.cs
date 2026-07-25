using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mediarq.Deferred;

/// <summary>
/// Drains the queue backing <see cref="IDeferredDispatcher"/>, running each queued dispatch in its own
/// DI scope. On a graceful host shutdown, <see cref="StopAsync"/> stops accepting new work and waits for
/// everything already queued to finish (bounded by the host's own shutdown timeout) instead of abandoning
/// it — the "reliable" half of "reliable in-process fire-and-forget".
/// </summary>
internal sealed class DeferredDispatchHostedService(
    Channel<Func<IServiceProvider, CancellationToken, Task>> channel,
    IServiceScopeFactory scopeFactory,
    ILogger<DeferredDispatchHostedService> logger)
    : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Deliberately not `stoppingToken`: shutdown is signaled by completing the channel writer (see
        // StopAsync), so this keeps draining already-queued work instead of aborting mid-item.
        await foreach (var work in channel.Reader.ReadAllAsync(CancellationToken.None).ConfigureAwait(false))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                await work(scope.ServiceProvider, CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "A deferred dispatch failed.");
            }
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Writer.TryComplete();
        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }
}
