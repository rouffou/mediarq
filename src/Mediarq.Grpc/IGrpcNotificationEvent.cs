using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Grpc;

/// <summary>
/// Marks a notification as a gRPC cross-service event — one meant to be forwarded, over gRPC, to another
/// service's <see cref="GrpcEndpointRouteBuilderExtensions.MapMediarqGrpcNotificationService"/> endpoint
/// (in addition to any in-process handlers) when published, and/or received back from gRPC into the real
/// Mediarq pipeline via <see cref="GrpcEndpointRouteBuilderExtensions.MapMediarqGrpcSubscription{TNotification}"/>.
/// </summary>
/// <remarks>
/// <see cref="ServiceAddress"/> is <c>static abstract</c> rather than an instance property (same rationale
/// as <c>IDaprPubSubEvent</c>/<c>IRabbitMqEvent</c>): the publish side needs this routing information
/// available before any notification instance exists, and reading it from the type rather than an
/// instance keeps it impossible for two instances of the same notification type to accidentally target
/// different remote services.
/// </remarks>
public interface IGrpcNotificationEvent : INotification
{
    /// <summary>The base address (e.g. <c>https://order-service:5001</c>) of the remote service's Mediarq
    /// gRPC notification endpoint to forward this notification type to.</summary>
    static abstract string ServiceAddress { get; }
}
