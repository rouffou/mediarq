namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Default <see cref="INotificationPublisher"/>: starts every handler and awaits them together with
/// <see cref="Task.WhenAll(System.Collections.Generic.IEnumerable{Task})"/>. Every handler is started,
/// even when an earlier one throws synchronously (that exception is captured into its task); if several
/// fail, the first exception is surfaced.
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
            try
            {
                tasks[i] = handlers[i](cancellationToken);
            }
            catch (Exception ex)
            {
                // A handler that throws before returning its Task must not prevent the others from starting.
                tasks[i] = Task.FromException(ex);
            }
        }

        return Task.WhenAll(tasks);
    }
}
