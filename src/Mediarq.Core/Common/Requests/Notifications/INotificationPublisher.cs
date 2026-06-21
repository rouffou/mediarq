namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Strategy that determines how the resolved notification handlers are invoked when a notification
/// is published (for example concurrently or sequentially).
/// </summary>
/// <remarks>
/// Register a custom implementation before calling <c>AddMediarq</c>/<c>AddMediarqCore</c> to override
/// the default (<see cref="ParallelNotificationPublisher"/>).
/// </remarks>
public interface INotificationPublisher
{
    /// <summary>
    /// Invokes the supplied handler callbacks for a published notification.
    /// </summary>
    /// <param name="handlers">The handler invocations to run. Each callback runs one notification handler.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that completes when the chosen strategy has finished invoking the handlers.</returns>
    Task Publish(IReadOnlyList<Func<CancellationToken, Task>> handlers, CancellationToken cancellationToken);
}
