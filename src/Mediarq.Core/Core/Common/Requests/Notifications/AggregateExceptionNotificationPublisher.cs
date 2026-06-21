namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// <see cref="INotificationPublisher"/> that starts every handler and, if one or more fail, throws an
/// <see cref="AggregateException"/> collecting <em>all</em> failures — unlike
/// <see cref="ParallelNotificationPublisher"/>, which surfaces only the first.
/// </summary>
/// <remarks>
/// Register this before <c>AddMediarq</c>/<c>AddMediarqCore</c> to observe every handler failure.
/// </remarks>
public sealed class AggregateExceptionNotificationPublisher : INotificationPublisher
{
    /// <inheritdoc />
    public async Task Publish(IReadOnlyList<Func<CancellationToken, Task>> handlers, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handlers);

        if (handlers.Count == 0)
        {
            return;
        }

        var tasks = new Task[handlers.Count];
        for (int i = 0; i < handlers.Count; i++)
        {
            try
            {
                tasks[i] = handlers[i](cancellationToken);
            }
            catch (Exception exception)
            {
                // A handler that throws synchronously should still be collected, not abort the loop.
                tasks[i] = Task.FromException(exception);
            }
        }

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch
        {
            // Task.WhenAll surfaces only the first exception; gather them all instead.
            var exceptions = tasks
                .Where(task => task.IsFaulted && task.Exception is not null)
                .SelectMany(task => task.Exception!.InnerExceptions)
                .ToList();

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            throw;
        }
    }
}
