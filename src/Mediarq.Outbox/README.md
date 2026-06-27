# Mediarq.Outbox

A transactional outbox over EF Core: stage a notification in the **same transaction** as your business
data, then publish it reliably afterwards — the event is never lost and never published without the data.

```bash
dotnet add package Mediarq.Outbox
```

## Usage

1. Map the outbox table in your `DbContext`:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
    => modelBuilder.ApplyMediarqOutbox();
```

2. Register it (after `AddMediarq` / `AddMediarqCore`, and after your `DbContext`):
```csharp
builder.Services.AddMediarqOutbox<AppDbContext>(o => o.PollingInterval = TimeSpan.FromSeconds(5));
```

3. Enqueue from a handler — typically a transactional command, so the unit of work commits the event with
   your data:
```csharp
public class CreateOrderHandler(AppDbContext db, IOutbox outbox)
    : ICommandHandler<CreateOrder, Result<Guid>>          // CreateOrder : ITransactionalRequest
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken ct = default)
    {
        var order = /* ... */;
        db.Orders.Add(order);
        outbox.Enqueue(new OrderPlaced(order.Id));        // committed atomically, published later
        return Task.FromResult(Result.Success(order.Id));
    }
}
```

A background `OutboxProcessor` publishes pending messages through `IPublisher` within the polling
interval (at-least-once). Pairs naturally with **Mediarq.EntityFrameworkCore** (the unit of work) and
**Mediarq.MassTransit** (forward the published event out-of-process).

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
