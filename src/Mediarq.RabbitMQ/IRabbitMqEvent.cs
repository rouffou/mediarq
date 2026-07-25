using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.RabbitMQ;

/// <summary>
/// Marks a notification as a RabbitMQ event — one meant to be published on an exchange (in addition to
/// any in-process handlers) so other services can consume it, and/or received back from a queue into the
/// real Mediarq pipeline via <see cref="RabbitMqServiceCollectionExtensions.AddMediarqRabbitMqSubscriber{TNotification}"/>.
/// </summary>
/// <remarks>
/// <see cref="Exchange"/>, <see cref="Queue"/> and <see cref="RoutingKey"/> are <c>static abstract</c>
/// rather than instance properties: the subscribe side needs this routing information at startup, before
/// any notification instance exists, and both directions reading the same static members means the
/// publish routing and the subscribe routing for a given event type can never drift apart — the same
/// rationale as <c>Mediarq.Dapr</c>'s <c>IDaprPubSubEvent</c>.
/// </remarks>
public interface IRabbitMqEvent : INotification
{
    /// <summary>The exchange to publish to / bind the queue to.</summary>
    static abstract string Exchange { get; }

    /// <summary>The queue a subscriber consumes from. Ignored when only publishing.</summary>
    static abstract string Queue { get; }

    /// <summary>The routing key to publish with / bind the queue on.</summary>
    static abstract string RoutingKey { get; }
}
