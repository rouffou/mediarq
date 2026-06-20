namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Optional contract for an <see cref="INotificationHandler{TNotification}"/> that needs a deterministic
/// execution order relative to the other handlers of the same notification.
/// </summary>
/// <remarks>
/// Handlers are ordered by ascending <see cref="Order"/> (lower runs first). Handlers that do not
/// implement this interface default to <see cref="int.MaxValue"/> and keep their registration order
/// (stable sort), so they run after ordered ones.
/// </remarks>
public interface IOrderedNotificationHandler
{
    /// <summary>Gets the execution order of this handler (lower runs first).</summary>
    int Order { get; }
}
