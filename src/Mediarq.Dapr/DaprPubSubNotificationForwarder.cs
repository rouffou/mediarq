using Dapr.Client;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Dapr;

/// <summary>
/// A Mediarq notification handler that forwards the notification to a Dapr pub/sub component via
/// <see cref="DaprClient.PublishEventAsync{TData}(string, string, TData, System.Threading.CancellationToken)"/>.
/// </summary>
/// <typeparam name="TNotification">The notification type to forward.</typeparam>
/// <remarks>
/// Registered as a regular <see cref="INotificationHandler{TNotification}"/>, so it runs alongside any
/// in-process handlers when the notification is published through Mediarq.
/// </remarks>
public sealed class DaprPubSubNotificationForwarder<TNotification> : INotificationHandler<TNotification>
    where TNotification : class, IDaprPubSubEvent
{
    private readonly DaprClient _daprClient;

    /// <summary>Initializes a new instance of the <see cref="DaprPubSubNotificationForwarder{TNotification}"/> class.</summary>
    /// <param name="daprClient">The Dapr client used to publish the event.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="daprClient"/> is <see langword="null"/>.</exception>
    public DaprPubSubNotificationForwarder(DaprClient daprClient)
    {
        ArgumentNullException.ThrowIfNull(daprClient);
        _daprClient = daprClient;
    }

    /// <inheritdoc />
    public Task Handle(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        return _daprClient.PublishEventAsync(TNotification.PubsubName, TNotification.Topic, notification, cancellationToken);
    }
}
