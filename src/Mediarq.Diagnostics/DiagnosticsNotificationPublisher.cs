using System.Diagnostics;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Diagnostics;

/// <summary>
/// Decorates an <see cref="INotificationPublisher"/> to record an <see cref="Activity"/> and metrics
/// around <c>Publish</c>. This is how notifications get observability, since they don't flow through
/// the request pipeline (the decorator sees the handler callbacks, not the concrete notification type).
/// </summary>
public sealed class DiagnosticsNotificationPublisher : INotificationPublisher
{
    private readonly INotificationPublisher _inner;

    /// <summary>Initializes the decorator around <paramref name="inner"/>.</summary>
    /// <param name="inner">The publisher to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="inner"/> is <see langword="null"/>.</exception>
    public DiagnosticsNotificationPublisher(INotificationPublisher inner)
    {
        ArgumentNullException.ThrowIfNull(inner);
        _inner = inner;
    }

    /// <inheritdoc />
    public async Task Publish(IReadOnlyList<Func<CancellationToken, Task>> handlers, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handlers);

        if (handlers.Count == 0)
        {
            await _inner.Publish(handlers, cancellationToken).ConfigureAwait(false);
            return;
        }

        using var activity = MediarqDiagnostics.ActivitySource.StartActivity("Mediarq:Publish", ActivityKind.Internal);
        activity?.SetTag("mediarq.handler_count", handlers.Count);

        var startTimestamp = Stopwatch.GetTimestamp();
        try
        {
            await _inner.Publish(handlers, cancellationToken).ConfigureAwait(false);
            MediarqDiagnostics.Record("Publish", Stopwatch.GetElapsedTime(startTimestamp), succeeded: true);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception exception)
        {
            MediarqDiagnostics.Record("Publish", Stopwatch.GetElapsedTime(startTimestamp), succeeded: false);
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
    }
}
