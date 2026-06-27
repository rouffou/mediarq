# Mediarq.UnitOfWork

Commit a unit of work around a command that implements `ITransactionalRequest`. The behavior calls
`IUnitOfWork.SaveChangesAsync()` **after** the handler, and **only when the result is not a failure** —
so a handler just stages changes and a failed `Result` rolls nothing forward.

```bash
dotnet add package Mediarq.UnitOfWork
```

## Usage

Implement `IUnitOfWork` over your data access and register it:

```csharp
builder.Services.AddScoped<IUnitOfWork, MyUnitOfWork>();
builder.Services.AddMediarqUnitOfWork(); // after AddMediarq / AddMediarqCore
```

```csharp
public record CreateOrder(string Customer) : ICommand<Result<Guid>>, ITransactionalRequest;

public class CreateOrderHandler(IRepository repo) : ICommandHandler<CreateOrder, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken ct = default)
    {
        var id = repo.Add(/* ... */);     // staged; no SaveChanges here
        return Task.FromResult(Result.Success(id));
    }
}
```

> Using EF Core? **Mediarq.EntityFrameworkCore** wires your `DbContext` as the `IUnitOfWork` in one call.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
