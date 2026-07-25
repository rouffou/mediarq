using System.Text.Json;
using Mediarq.Core.Common.Requests.Notifications;
using RabbitMQ.Client;

namespace Mediarq.RabbitMQ;

/// <summary>
/// A Mediarq notification handler that forwards the notification to its RabbitMQ exchange/routing key,
/// JSON-serialized, on a short-lived channel created per publish.
/// </summary>
/// <typeparam name="TNotification">The notification type to forward.</typeparam>
/// <remarks>
/// Registered as a regular <see cref="INotificationHandler{TNotification}"/>, so it runs alongside any
/// in-process handlers when the notification is published through Mediarq. Requires an <see cref="IConnection"/>
/// already registered in DI — this package never owns the connection's lifecycle.
/// </remarks>
public sealed class RabbitMqNotificationForwarder<TNotification> : INotificationHandler<TNotification>
    where TNotification : class, IRabbitMqEvent
{
    private readonly IConnection _connection;

    /// <summary>Initializes a new instance of the <see cref="RabbitMqNotificationForwarder{TNotification}"/> class.</summary>
    /// <param name="connection">The RabbitMQ connection a per-publish channel is created from.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="connection"/> is <see langword="null"/>.</exception>
    public RabbitMqNotificationForwarder(IConnection connection)
    {
        ArgumentNullException.ThrowIfNull(connection);
        _connection = connection;
    }

    /// <inheritdoc />
    public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var body = JsonSerializer.SerializeToUtf8Bytes(notification);
        var properties = new BasicProperties { ContentType = "application/json" };
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        await channel.BasicPublishAsync(TNotification.Exchange, TNotification.RoutingKey, mandatory: false, properties, body, cancellationToken)
            .ConfigureAwait(false);
    }
}
