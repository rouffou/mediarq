# Mediarq.Saga

Saga / process-manager primitives: persisted, **correlated state across a sequence of notifications**.
Each step is a regular notification handler that loads its instance's state, mutates it, and saves it
back — no orchestration DSL, no new dispatch pipeline.

```bash
dotnet add package Mediarq.Saga
```

## Usage

```csharp
builder.Services.AddMediarqSaga<OrderFulfillmentState>();
```

```csharp
public sealed class OrderFulfillmentState : ISagaState
{
    public required Guid CorrelationId { get; init; }
    public bool IsComplete { get; set; }
    public bool PaymentReceived { get; set; }
}

public sealed class OnOrderPlaced(ISagaStore<OrderFulfillmentState> store)
    : SagaNotificationHandler<OrderPlacedEvent, OrderFulfillmentState>(store)
{
    protected override Guid GetCorrelationId(OrderPlacedEvent e) => e.OrderId;
    protected override OrderFulfillmentState CreateState(Guid correlationId) => new() { CorrelationId = correlationId };

    protected override Task HandleAsync(OrderPlacedEvent e, OrderFulfillmentState state, CancellationToken ct)
    {
        // e.g. outbox.Enqueue(new RequestPaymentCommand(e.OrderId)); — Mediarq.Outbox delivers it reliably.
        return Task.CompletedTask;
    }
}

public sealed class OnPaymentReceived(ISagaStore<OrderFulfillmentState> store)
    : SagaNotificationHandler<PaymentReceivedEvent, OrderFulfillmentState>(store)
{
    protected override Guid GetCorrelationId(PaymentReceivedEvent e) => e.OrderId;
    protected override OrderFulfillmentState CreateState(Guid correlationId) => new() { CorrelationId = correlationId };

    protected override Task HandleAsync(PaymentReceivedEvent e, OrderFulfillmentState state, CancellationToken ct)
    {
        state.PaymentReceived = true;
        state.IsComplete = true; // further notifications for this correlation id are ignored
        return Task.CompletedTask;
    }
}
```

`OnOrderPlaced` and `OnPaymentReceived` both resolve the same `CorrelationId` (the order id) from their
respective events, so they read and write the *same* persisted `OrderFulfillmentState` instance across
two separate notification dispatches — that shared, correlated state is what makes this a process
manager rather than two unrelated handlers.

## Pairing with the outbox

A step usually needs to trigger the next one. Enqueue that follow-up via
[`Mediarq.Outbox`](https://www.nuget.org/packages/Mediarq.Outbox)'s `IOutbox.Enqueue` from inside
`HandleAsync` — it is staged on the same unit of work and only published once the transaction commits, so
a crash between steps can't lose the trigger the way an in-process `Publish` call could.

## Persistent stores

`AddMediarqSaga<TState>()` registers `InMemorySagaStore<TState>` — process-lifetime only, lost on
restart. Register your own `ISagaStore<TState>` (e.g. backed by a database via
`Mediarq.EntityFrameworkCore`) **before** calling `AddMediarqSaga<TState>()` to use it instead; the
extension only fills in a default when nothing is already registered.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
