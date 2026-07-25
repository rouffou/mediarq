# Mediarq.Grpc

gRPC transport for cross-service Mediarq notifications: forward a notification to another service's
Mediarq gRPC endpoint when it is published, and receive it back into the real Mediarq pipeline via a
single shared gRPC service. Ships its own compiled Protobuf/gRPC contract — reference this package only,
no `protoc`/`Grpc.Tools` required on your side.

```bash
dotnet add package Mediarq.Grpc
```

## Marking an event

```csharp
public sealed record OrderPlaced(Guid OrderId, string Customer) : IGrpcNotificationEvent
{
    public static string ServiceAddress => "https://order-service:5001";
}
```

`ServiceAddress` is a `static abstract` member, not an instance property: the publish side needs this
routing information available before any `OrderPlaced` instance exists, and reading it from the type
rather than an instance keeps it impossible for two instances of the same notification type to
accidentally target different remote services.

## Publishing (outbound)

```csharp
builder.Services.AddMediarqGrpcPublisher<OrderPlaced>();
```

Now every `IPublisher.Publish(new OrderPlaced(...))` call also calls `OrderPlaced.ServiceAddress`'s
`NotificationService.Publish` RPC, in addition to running any in-process handlers. The HTTP/2 channel to
each `ServiceAddress` is cached and reused (not one per publish) and disposed when the app shuts down —
unlike the Dapr/RabbitMQ/Azure Service Bus packages, this package owns that channel's lifecycle itself,
since (unlike a Dapr sidecar client, a RabbitMQ connection, or a Service Bus client) it carries no
external credentials your app needs to configure.

## Subscribing (inbound)

```csharp
builder.Services.AddMediarqGrpcSubscriptions();
// ...
app.MapMediarqGrpcNotificationService();     // map exactly once
app.MapMediarqGrpcSubscription<OrderPlaced>();
```

Every subscribed notification type is multiplexed over the single shared gRPC service — there is one RPC
method (`Publish`), and the envelope's `type_name` field tells the receiving service which registered
type to deserialize the JSON payload as and republish through `IPublisher`. A gRPC call for a type that
was never subscribed on this service fails with `StatusCode.NotFound`.

## What this does *not* do

Unlike a message broker (Dapr pub/sub, RabbitMQ, Azure Service Bus), gRPC is direct point-to-point RPC:
the publisher must know the target service's network address, there is no fan-out, no persistence, and
no retry/redelivery if the remote service is unreachable — a failed `Publish` RPC throws
`Grpc.Core.RpcException`, same as any other Mediarq notification handler exception. If you need broker
semantics (decoupled publisher/subscriber, delivery guarantees, dead-lettering), use one of the other
transport packages instead.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme) ·
[gRPC for .NET docs](https://learn.microsoft.com/en-us/aspnet/core/grpc/)

MIT © Nicolas Rouffart
