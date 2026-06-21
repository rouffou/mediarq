# Mediarq.MassTransit

Forward Mediarq notifications onto a [MassTransit](https://masstransit.io) bus, so other services can
consume them out-of-process — in addition to your in-process handlers.

```bash
dotnet add package Mediarq.MassTransit
```

## Usage

```csharp
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx)); // or RabbitMQ / Azure Service Bus
});

builder.Services.AddMediarqMassTransitForwarding<OrderPlaced>();              // one event, or
builder.Services.AddMediarqMassTransitForwarding(typeof(OrderPlaced).Assembly); // every IIntegrationEvent
```

```csharp
public record OrderPlaced(Guid Id) : IIntegrationEvent; // IIntegrationEvent : INotification
```

Now `await publisher.Publish(new OrderPlaced(id))` runs the in-process handlers **and** publishes the
event on the bus. The forwarder is a regular notification handler. Pairs well with **Mediarq.Outbox** for
reliable, transactional publication.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
