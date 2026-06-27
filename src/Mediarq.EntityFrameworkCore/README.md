# Mediarq.EntityFrameworkCore

Use your EF Core `DbContext` as the Mediarq unit of work. One call registers it as the `IUnitOfWork` and
wires the behavior that commits after a successful `ITransactionalRequest`.

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

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
