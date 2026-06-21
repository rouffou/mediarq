# Mediarq.Core

The core of **Mediarq** — a lightweight, dependency-free CQRS mediator for .NET: commands, queries,
no-result commands, notifications, streaming, a composable pipeline of behaviors, built-in validation
and `Result` types. Includes the optional source generator for reflection-free, Native-AOT-friendly
dispatch.

```bash
dotnet add package Mediarq.Core
```

> Want the core **plus** the lightweight extensions in one install? Use the **`Mediarq`** meta-package.

## Quick start

```csharp
using Mediarq.Extensions;

builder.Services.AddLogging();
builder.Services.AddMediarq(isHttp: false, typeof(CreateOrder).Assembly); // scan for handlers
```

```csharp
public record CreateOrder(string Customer) : ICommand<Result<Guid>>;

public class CreateOrderHandler : ICommandHandler<CreateOrder, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken ct = default)
        => Task.FromResult(Result.Success(Guid.NewGuid()));
}

// inject IMediator / ISender / IPublisher
Result<Guid> result = await sender.Send(new CreateOrder("Alice"));
```

- **Commands / queries** return a `Result` / `Result<T>` (railway-oriented); void commands implement `ICommand`.
- **Notifications** (`INotification`) fan out to zero or more handlers.
- **Streaming** (`IStreamRequest<T>`) returns `IAsyncEnumerable<T>` via `CreateStream`.
- **Pipeline behaviors** wrap the handler; built-in validation/exception/pre/post light up automatically.

## Reflection-free / Native AOT

```csharp
builder.Services.AddMediarqCore(isHttp: false).AddMediarqHandlers(); // generated at compile time
```

No assembly scan, no reflection on dispatch. The library is `IsAotCompatible` and publishes cleanly with
Native AOT.

## Learn more

- [Concepts](https://github.com/rouffou/mediarq/blob/main/docs/guides/concepts.md) ·
  [Your first app](https://github.com/rouffou/mediarq/blob/main/docs/guides/your-first-app.md) ·
  [Troubleshooting](https://github.com/rouffou/mediarq/blob/main/docs/guides/troubleshooting.md)
- [Full README & extension packages](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
