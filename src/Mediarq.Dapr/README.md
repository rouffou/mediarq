# Mediarq.Dapr

Dapr pub/sub integration for Mediarq: publish a notification onto a Dapr pub/sub component when it is
published through Mediarq, and receive Dapr-delivered messages back into the real Mediarq pipeline via a
minimal-API webhook — including the `/dapr/subscribe` discovery endpoint the Dapr sidecar queries at
startup.

```bash
dotnet add package Mediarq.Dapr
dotnet add package Dapr.AspNetCore   # AddDaprClient(), if you don't already have it
```

## Marking an event

```csharp
public sealed record OrderPlaced(Guid OrderId, string Customer) : IDaprPubSubEvent
{
    public static string PubsubName => "orders-pubsub";
    public static string Topic => "order-placed";
}
```

`PubsubName`/`Topic` are `static abstract` members, not instance properties: the subscribe side needs
this routing information at startup, before any `OrderPlaced` instance exists, and both directions
reading the same static members means the publish topic and the subscribe topic can never drift apart.

## Publishing (outbound)

```csharp
builder.Services.AddDaprClient();                  // from Dapr.AspNetCore/Dapr.Client
builder.Services.AddMediarqDaprPubSub<OrderPlaced>();
```

Now every `IPublisher.Publish(new OrderPlaced(...))` call also publishes `OrderPlaced` on the
`orders-pubsub` component's `order-placed` topic, in addition to running any in-process handlers.

## Subscribing (inbound)

```csharp
builder.Services.AddMediarqDaprPubSubSubscriptions();
// ...
app.MapDaprPubSubSubscription<OrderPlaced>();   // POST /dapr/pubsub/OrderPlaced
app.MapDaprPubSubSubscribeEndpoint();           // GET  /dapr/subscribe (map after every subscription)
```

The sidecar calls `GET /dapr/subscribe` at startup to discover `{pubsubname, topic, route}` for every
subscribed type, then POSTs matching messages to the mapped route as a CloudEvents 1.0 envelope. This
package extracts the `data` field and republishes it through `IPublisher` — the same pipeline as any
in-process `Publish` call (validation, other behaviors, every registered handler for the type).

A route override is available for when the default (`/dapr/pubsub/{TypeName}`) collides with something else:

```csharp
app.MapDaprPubSubSubscription<OrderPlaced>("/webhooks/order-placed");
```

## What this does *not* do

This is not a full CloudEvents implementation — only the `data` field is read, every other envelope
field (`id`, `source`, `type`, ...) is ignored. It also only covers pub/sub, not Dapr's other building
blocks (state store, bindings, service invocation, secrets) — inject `Dapr.Client.DaprClient` directly
for those, same as in a non-Mediarq Dapr app.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme) ·
[Dapr pub/sub docs](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/)

MIT © Nicolas Rouffart
