using MassTransit;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.MassTransit;

/// <summary>
/// A Mediarq notification handler that forwards the notification to a MassTransit
/// <see cref="IPublishEndpoint"/>, publishing it on the bus for out-of-process consumers.
/// </summary>
/// <typeparam name="TNotification">The notification type to forward.</typeparam>
/// <remarks>
/// Registered as a regular <see cref="INotificationHandler{TNotification}"/>, so it runs alongside
/// any in-process handlers when the notification is published through Mediarq.
/// </remarks>
public sealed class MassTransitNotificationForwarder<TNotification> : INotificationHandler<TNotification>
    where TNotification : class, INotification
{
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="MassTransitNotificationForwarder{TNotification}"/> class.
    /// </summary>
    /// <param name="publishEndpoint">The MassTransit publish endpoint used to publish the notification.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="publishEndpoint"/> is <see langword="null"/>.</exception>
    public MassTransitNotificationForwarder(IPublishEndpoint publishEndpoint)
    {
        ArgumentNullException.ThrowIfNull(publishEndpoint);
        _publishEndpoint = publishEndpoint;
    }

    /// <inheritdoc />
    public Task Handle(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return _publishEndpoint.Publish(notification, cancellationToken);
    }
}
