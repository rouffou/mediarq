using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Publishes notifications to their handlers. This is the notification half of the mediator; inject it
/// when a component only needs to publish notifications.
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <remarks>
    /// How the handlers are invoked (concurrently, sequentially, ...) is determined by the registered
    /// <see cref="INotificationPublisher"/>. Publishing a notification that has no registered handler
    /// completes successfully without doing anything.
    /// </remarks>
    /// <typeparam name="TNotification">The notification type. Must implement <see cref="INotification"/>.</typeparam>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="notification"/> is <see langword="null"/>.</exception>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
}
