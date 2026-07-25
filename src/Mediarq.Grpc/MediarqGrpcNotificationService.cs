using Grpc.Core;
using Mediarq.Core.Mediators;
using Mediarq.Grpc.Contracts;

namespace Mediarq.Grpc;

/// <summary>
/// The single shared gRPC service every subscribed notification type is multiplexed through: looks up
/// <see cref="NotificationEnvelope.TypeName"/> in the <see cref="GrpcNotificationSubscriptionRegistry"/>,
/// deserializes the payload, and republishes it through the real Mediarq pipeline via
/// <see cref="IPublisher"/>. Constructed per call by ASP.NET Core's gRPC hosting, in that call's own DI
/// scope, so <see cref="IPublisher"/> (Scoped) resolves correctly without any manual scope creation.
/// </summary>
internal sealed class MediarqGrpcNotificationService(GrpcNotificationSubscriptionRegistry registry, IPublisher publisher)
    : NotificationService.NotificationServiceBase
{
    /// <inheritdoc />
    public override async Task<PublishAck> Publish(NotificationEnvelope request, ServerCallContext context)
    {
        if (!registry.TryGetHandler(request.TypeName, out var handler))
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"No subscriber registered for notification type '{request.TypeName}'."));
        }

        await handler(request.Payload, publisher, context.CancellationToken).ConfigureAwait(false);
        return new PublishAck { Accepted = true };
    }
}
