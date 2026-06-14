using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Non-generic entry point the <see cref="Mediator"/> calls to publish a notification.
/// Implementations are cached per concrete notification type.
/// </summary>
internal interface INotificationHandlerWrapper
{
    Task Handle(
        object notification,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken);
}

/// <summary>
/// Closed over the concrete notification type. Because <typeparamref name="TNotification"/> is known,
/// handler resolution and invocation are strongly-typed — no <c>GetMethod</c>/<c>Invoke</c> reflection
/// on the publish path.
/// </summary>
/// <typeparam name="TNotification">The concrete notification type.</typeparam>
internal sealed class NotificationHandlerWrapperImpl<TNotification> : INotificationHandlerWrapper
    where TNotification : INotification
{
    public Task Handle(
        object notification,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken)
    {
        var typedNotification = (TNotification)notification;

        IReadOnlyList<INotificationHandler<TNotification>> handlers =
            handlerResolver.ResolveAll<INotificationHandler<TNotification>>();

        if (handlers.Count == 0)
        {
            // Publishing a notification with no registered handler is a no-op.
            return Task.CompletedTask;
        }

        var callbacks = new List<Func<CancellationToken, Task>>(handlers.Count);
        foreach (var handler in handlers)
        {
            var captured = handler;
            callbacks.Add(ct => captured.Handle(typedNotification, ct));
        }

        // The configured INotificationPublisher decides how handlers are invoked (parallel, sequential, ...).
        return notificationPublisher.Publish(callbacks, cancellationToken);
    }
}
