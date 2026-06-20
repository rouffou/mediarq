namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Marker interface representing a notification that can be published to zero or more
/// <see cref="INotificationHandler{TNotification}"/> implementations.
/// </summary>
/// <remarks>
/// Unlike commands and queries, a notification has no return value and may be handled by
/// any number of handlers. Publishing a notification with no registered handler is a no-op.
/// </remarks>
public interface INotification;
