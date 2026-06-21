namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Default <see cref="INotificationPublisher"/>: starts every handler and awaits them together with
/// <see cref="Task.WhenAll(System.Collections.Generic.IEnumerable{Task})"/>. Every handler is started;
/// if several fail, the first exception is surfaced.
/// </summary>
public sealed class ParallelNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public Task Publish(IReadOnlyList<Func<CancellationToken, Task>> handlers, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handlers);

        var tasks = new Task[handlers.Count];
        for (int i = 0; i < handlers.Count; i++)
        {
            tasks[i] = handlers[i](cancellationToken);
        }

        return Task.WhenAll(tasks);
    }
}
