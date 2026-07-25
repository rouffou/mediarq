# Mediarq.EntityFrameworkCore

Use your EF Core `DbContext` as the Mediarq unit of work, and publish domain events raised by your
aggregates after a successful commit.

```bash
dotnet add package Mediarq.EntityFrameworkCore
```

## Usage

```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString)); // your context first
builder.Services.AddMediarqEntityFrameworkCore<AppDbContext>();                     // after AddMediarq/AddMediarqCore
```

```csharp
public record CreateOrder(string Customer) : ICommand<Result<Guid>>, ITransactionalRequest;

public class CreateOrderHandler(AppDbContext db) : ICommandHandler<CreateOrder, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken ct = default)
    {
        var order = new Order { /* ... */ };
        db.Orders.Add(order);                       // staged; the behavior calls SaveChanges on success
        return Task.FromResult(Result.Success(order.Id));
    }
}
```

A failed `Result` is not persisted. Combine with **Mediarq.Outbox** to commit integration events
atomically with your data.

## Domain events

```csharp
builder.Services.AddMediarqDomainEvents(); // registers DomainEventsInterceptor for AppDbContext to pick up
```

```csharp
public class Order : AggregateRoot // or implement IHasDomainEvents yourself
{
    public Guid Id { get; private set; }

    public void Place()
    {
        // ... mutate state ...
        AddDomainEvent(new OrderPlaced(Id));
    }
}
```

The event is collected (and cleared from the aggregate) right before `SaveChangesAsync` runs, then
`Publish`ed only once the commit actually succeeds — a failed save discards the collected events rather
than publishing them or leaving them to be raised twice on a retry. Only `SaveChangesAsync` is
intercepted; there is no synchronous `SaveChanges` support (`IPublisher` is async-only).

No wiring needed in your own `AddDbContext(...)` call — the interceptor is picked up automatically from
the application service provider, the same way `AddDbContext` already resolves everything else it needs.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
