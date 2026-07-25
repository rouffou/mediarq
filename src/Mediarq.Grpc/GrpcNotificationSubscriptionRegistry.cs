using System.Collections.Concurrent;
using System.Text.Json;
using Google.Protobuf;
using Mediarq.Core.Mediators;

namespace Mediarq.Grpc;

/// <summary>
/// Maps a notification's <see cref="Type.FullName"/> (as carried by a received
/// <see cref="Contracts.NotificationEnvelope.TypeName"/>) to a compile-time-typed deserialize-and-publish
/// delegate, so <see cref="MediarqGrpcNotificationService"/> can dispatch every subscribed notification
/// type through the single shared <c>Publish</c> RPC without any runtime reflection.
/// </summary>
/// <remarks>
/// Populated by <see cref="GrpcEndpointRouteBuilderExtensions.MapMediarqGrpcSubscription{TNotification}"/>
/// at endpoint-mapping time (mirroring how <c>Mediarq.Dapr</c>'s subscription registry is populated),
/// rather than at <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/> configuration
/// time, since only then does the app's <see cref="IServiceProvider"/> exist to resolve this singleton.
/// </remarks>
internal sealed class GrpcNotificationSubscriptionRegistry
{
    private readonly ConcurrentDictionary<string, Func<ByteString, IPublisher, CancellationToken, Task>> _handlers = new();

    internal void Add<TNotification>()
        where TNotification : class, IGrpcNotificationEvent
    {
        _handlers[typeof(TNotification).FullName!] = static (payload, publisher, cancellationToken) =>
        {
            var notification = JsonSerializer.Deserialize<TNotification>(payload.Span)
                ?? throw new InvalidOperationException(
                    $"Failed to deserialize a gRPC-delivered '{typeof(TNotification).Name}' notification: the payload deserialized to null.");

            return publisher.Publish(notification, cancellationToken);
        };
    }

    internal bool TryGetHandler(string typeName, out Func<ByteString, IPublisher, CancellationToken, Task> handler)
        => _handlers.TryGetValue(typeName, out handler!);
}
