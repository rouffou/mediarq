namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// <see cref="INotificationPublisher"/> that invokes the handlers one after another, in order, awaiting
/// each before starting the next. Propagates the first exception and stops.
/// </summary>
public sealed class SequentialNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public async Task Publish(IReadOnlyList<Func<CancellationToken, Task>> handlers, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handlers);

        foreach (var handler in handlers)
        {
            await handler(cancellationToken).ConfigureAwait(false);
        }
    }
}
