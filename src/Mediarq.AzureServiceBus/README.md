# Mediarq.AzureServiceBus

A lightweight, direct Azure Service Bus bridge for Mediarq notifications: publish a notification onto a
topic when it is published through Mediarq, and receive it back into the real Mediarq pipeline via a
background processor — no `Mediarq.MassTransit` dependency required for the simple pub/sub case.

```bash
dotnet add package Mediarq.AzureServiceBus
```

## Marking an event

```csharp
public sealed record OrderPlaced(Guid OrderId, string Customer) : IAzureServiceBusEvent
{
    public static string TopicName => "orders";
    public static string SubscriptionName => "order-placed-subscribers";
}
```

`TopicName`/`SubscriptionName` are `static abstract` members, not instance properties: the subscribe
side needs this routing information at startup, before any `OrderPlaced` instance exists, and both
directions reading the same static members means the publish topic and the subscribe topic/subscription
can never drift apart (same rationale as `Mediarq.Dapr`'s `IDaprPubSubEvent` and `Mediarq.RabbitMQ`'s
`IRabbitMqEvent`).

This package does **not** provision the topic/subscription — create them ahead of time (portal, ARM/Bicep,
or `ServiceBusAdministrationClient`), the same way you would for any Service Bus application.

## Bring your own client

This package never owns the client's lifecycle — register a `ServiceBusClient` yourself:

```csharp
builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));
// or, with Microsoft Entra ID: new ServiceBusClient(fullyQualifiedNamespace, new DefaultAzureCredential())
```

## Publishing (outbound)

```csharp
builder.Services.AddMediarqAzureServiceBusPublisher<OrderPlaced>();
```

Every `IPublisher.Publish(new OrderPlaced(...))` call now also JSON-serializes `OrderPlaced` and sends it
to the `orders` topic — on a sender created per publish — in addition to running any in-process handlers.

## Subscribing (inbound)

```csharp
builder.Services.AddMediarqAzureServiceBusSubscriber<OrderPlaced>();
```

Processes the `order-placed-subscribers` subscription on the `orders` topic, deserializes each message
and republishes it through `IPublisher` — the same pipeline as any in-process `Publish` call (validation,
other behaviors, every registered handler). A processing failure (bad JSON, a throwing handler) is logged
and the message is moved straight to the dead-letter subqueue — the Service Bus analogue of "nack without
requeue" — so a poison message doesn't loop forever.

## Duplicate delivery

Service Bus's default (non-session, `PeekLock`) mode is at-least-once: a lock expiring before completion
lands can cause a redelivery. This package does not implement deduplication — enable the entity's built-in
duplicate detection window if you need broker-side dedup, or dedupe on a message id yourself if your
handlers aren't naturally idempotent.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
