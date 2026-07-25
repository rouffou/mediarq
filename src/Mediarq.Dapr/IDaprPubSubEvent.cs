using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Dapr;

/// <summary>
/// Marks a notification as a Dapr pub/sub event — one meant to be published on a Dapr pub/sub component
/// (in addition to any in-process handlers) so other services can consume it, and/or received back from
/// Dapr into the real Mediarq pipeline via <see cref="DaprPubSubEndpointRouteBuilderExtensions.MapDaprPubSubSubscription{TNotification}"/>.
/// </summary>
/// <remarks>
/// <see cref="PubsubName"/> and <see cref="Topic"/> are <c>static abstract</c> rather than instance
/// properties (unlike, say, <c>IRateLimitedRequest.PolicyName</c>): the subscribe side needs this routing
/// information at startup, before any notification instance — implicit in an instance member — exists.
/// Keeping both directions read the same static members also means the publish topic and the subscribe
/// topic for a given event type can never drift apart.
/// </remarks>
public interface IDaprPubSubEvent : INotification
{
    /// <summary>The Dapr pub/sub component name (as configured for the Dapr sidecar) to publish to / subscribe on.</summary>
    static abstract string PubsubName { get; }

    /// <summary>The topic name within <see cref="PubsubName"/> to publish to / subscribe on.</summary>
    static abstract string Topic { get; }
}
