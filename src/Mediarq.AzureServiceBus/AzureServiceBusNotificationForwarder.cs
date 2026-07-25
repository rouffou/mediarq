using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.AzureServiceBus;

/// <summary>
/// A Mediarq notification handler that forwards the notification to its Azure Service Bus topic,
/// JSON-serialized, on a sender created per publish.
/// </summary>
/// <typeparam name="TNotification">The notification type to forward.</typeparam>
/// <remarks>
/// Registered as a regular <see cref="INotificationHandler{TNotification}"/>, so it runs alongside any
/// in-process handlers when the notification is published through Mediarq. Requires a <see cref="ServiceBusClient"/>
/// already registered in DI — this package never owns the client's lifecycle.
/// </remarks>
public sealed class AzureServiceBusNotificationForwarder<TNotification> : INotificationHandler<TNotification>
    where TNotification : class, IAzureServiceBusEvent
{
    private readonly ServiceBusClient _client;

    /// <summary>Initializes a new instance of the <see cref="AzureServiceBusNotificationForwarder{TNotification}"/> class.</summary>
    /// <param name="client">The Service Bus client a per-publish sender is created from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> is <see langword="null"/>.</exception>
    public AzureServiceBusNotificationForwarder(ServiceBusClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
    }

    /// <inheritdoc />
    public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var body = JsonSerializer.SerializeToUtf8Bytes(notification);
        var message = new ServiceBusMessage(BinaryData.FromBytes(body)) { ContentType = "application/json" };

        await using var sender = _client.CreateSender(TNotification.TopicName);
        await sender.SendMessageAsync(message, cancellationToken).ConfigureAwait(false);
    }
}
