namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Defines a handler for a notification of type <typeparamref name="TNotification"/>.
/// </summary>
/// <typeparam name="TNotification">
/// The type of notification handled. Must implement <see cref="INotification"/>.
/// </typeparam>
/// <remarks>
/// A single notification may be handled by multiple <see cref="INotificationHandler{TNotification}"/>
/// implementations. All registered handlers are invoked when the notification is published.
/// </remarks>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles the specified <paramref name="notification"/>.
    /// </summary>
    /// <param name="notification">The notification to handle.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}
