# Mediarq.RabbitMQ

A lightweight, direct RabbitMQ bridge for Mediarq notifications: publish a notification onto an exchange
when it is published through Mediarq, and receive it back into the real Mediarq pipeline via a background
consumer — no `Mediarq.MassTransit` dependency required for the simple pub/sub case.

```bash
dotnet add package Mediarq.RabbitMQ
```

## Marking an event

```csharp
public sealed record OrderPlaced(Guid OrderId, string Customer) : IRabbitMqEvent
{
    public static string Exchange => "orders";
    public static string Queue => "orders.order-placed";
    public static string RoutingKey => "order-placed";
}
```

`Exchange`/`Queue`/`RoutingKey` are `static abstract` members, not instance properties: the subscribe
side needs this routing information at startup, before any `OrderPlaced` instance exists, and both
directions reading the same static members means the publish routing and the subscribe routing can never
drift apart (same rationale as `Mediarq.Dapr`'s `IDaprPubSubEvent`).

## Bring your own connection

This package never owns the connection's lifecycle — register an `IConnection` yourself:

```csharp
builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});
```

## Publishing (outbound)

```csharp
builder.Services.AddMediarqRabbitMqPublisher<OrderPlaced>();
```

Every `IPublisher.Publish(new OrderPlaced(...))` call now also JSON-serializes `OrderPlaced` and publishes
it to the `orders` exchange under the `order-placed` routing key — on a short-lived channel created per
publish — in addition to running any in-process handlers.

## Subscribing (inbound)

```csharp
builder.Services.AddMediarqRabbitMqSubscriber<OrderPlaced>();
```

Declares the exchange/queue/binding, consumes with manual acknowledgement, deserializes each delivery and
republishes it through `IPublisher` — the same pipeline as any in-process `Publish` call (validation,
other behaviors, every registered handler). A processing failure (bad JSON, a throwing handler) is logged
and the delivery is negatively acknowledged **without requeue**, so a poison message doesn't loop forever;
route it to a dead-letter exchange at the queue-argument level at the broker if you need one.

## Duplicate delivery

RabbitMQ is at-least-once: a connection drop between processing a message and its ack landing can cause a
redelivery. This package does not implement deduplication — if your handlers aren't naturally idempotent,
dedupe on a message id yourself (a first-class inbox built on `Mediarq.Idempotency` is a natural follow-up,
not bundled here).

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
