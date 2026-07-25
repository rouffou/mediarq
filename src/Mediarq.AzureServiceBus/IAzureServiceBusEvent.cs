using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.AzureServiceBus;

/// <summary>
/// Marks a notification as an Azure Service Bus event — one meant to be published on a topic (in
/// addition to any in-process handlers) so other services can consume it via a subscription, and/or
/// received back into the real Mediarq pipeline via
/// <see cref="AzureServiceBusServiceCollectionExtensions.AddMediarqAzureServiceBusSubscriber{TNotification}"/>.
/// </summary>
/// <remarks>
/// <see cref="TopicName"/> and <see cref="SubscriptionName"/> are <c>static abstract</c> rather than
/// instance properties: the subscribe side needs this routing information at startup, before any
/// notification instance exists, and both directions reading the same static members means the publish
/// topic and the subscribe topic/subscription can never drift apart — the same rationale as
/// <c>Mediarq.Dapr</c>'s <c>IDaprPubSubEvent</c> and <c>Mediarq.RabbitMQ</c>'s <c>IRabbitMqEvent</c>.
/// This package does not provision the topic/subscription — create them ahead of time (portal, ARM/Bicep,
/// or <c>ServiceBusAdministrationClient</c>), the same way you would for any Service Bus application.
/// </remarks>
public interface IAzureServiceBusEvent : INotification
{
    /// <summary>The topic to publish to / subscribe on.</summary>
    static abstract string TopicName { get; }

    /// <summary>The subscription within <see cref="TopicName"/> a subscriber consumes from. Ignored when only publishing.</summary>
    static abstract string SubscriptionName { get; }
}
