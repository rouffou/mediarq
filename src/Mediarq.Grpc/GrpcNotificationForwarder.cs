using System.Text.Json;
using Google.Protobuf;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Grpc.Contracts;

namespace Mediarq.Grpc;

/// <summary>
/// A Mediarq notification handler that forwards the notification, JSON-serialized inside a
/// <see cref="NotificationEnvelope"/>, to another service's Mediarq gRPC notification endpoint at
/// <see cref="IGrpcNotificationEvent.ServiceAddress"/>.
/// </summary>
/// <typeparam name="TNotification">The notification type to forward.</typeparam>
/// <remarks>
/// Registered as a regular <see cref="INotificationHandler{TNotification}"/>, so it runs alongside any
/// in-process handlers when the notification is published through Mediarq.
/// </remarks>
public sealed class GrpcNotificationForwarder<TNotification> : INotificationHandler<TNotification>
    where TNotification : class, IGrpcNotificationEvent
{
    private static readonly string TypeName = typeof(TNotification).FullName!;

    private readonly GrpcChannelCache _channelCache;

    /// <summary>Initializes a new instance of the <see cref="GrpcNotificationForwarder{TNotification}"/> class.</summary>
    /// <param name="channelCache">The channel cache used to reach <see cref="IGrpcNotificationEvent.ServiceAddress"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="channelCache"/> is <see langword="null"/>.</exception>
    public GrpcNotificationForwarder(GrpcChannelCache channelCache)
    {
        ArgumentNullException.ThrowIfNull(channelCache);
        _channelCache = channelCache;
    }

    /// <inheritdoc />
    public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var channel = _channelCache.GetOrCreate(TNotification.ServiceAddress);
        var client = new NotificationService.NotificationServiceClient(channel);
        var envelope = new NotificationEnvelope
        {
            TypeName = TypeName,
            Payload = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(notification)),
        };

        await client.PublishAsync(envelope, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
