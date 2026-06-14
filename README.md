# Mediarq

A lightweight, dependency-free **CQRS mediator for .NET** — a free alternative to MediatR with
commands, queries, no-result commands, notifications, a composable pipeline of behaviors, and
built-in validation and `Result` types. Designed for domain-driven and CQRS architectures.

- ✅ Commands / queries returning a `Result` (railway-oriented)
- ✅ No-result (void) commands routed through the same pipeline
- ✅ Notifications published to multiple handlers
- ✅ Pipeline behaviors (logging, performance, validation) + your own, orderable
- ✅ Built-in validation abstraction and `Result` / `ResultError` types
- ✅ No `dynamic` on the dispatch path — strongly-typed wrappers cached per request type
- ✅ Optional **source generator** for compile-time, reflection-free registration (trimming/AOT friendly)
- ✅ Functional `Result` combinators (`Map`, `Bind`, `Match`, `Tap`, `Ensure`)

> Targets **.NET 8, .NET 9 and .NET 10**.

---

## Installation

```bash
dotnet add package Mediarq
```

## Getting started

Register Mediarq, passing the assemblies that contain your handlers, behaviors and validators:

```csharp
using Mediarq.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddMediarq(isHttp: true, typeof(CreateUserCommand).Assembly);
// `isHttp: true` registers HttpUserContext — remember to also add the HTTP context accessor:
builder.Services.AddHttpContextAccessor();
```

`AddMediarq` parameters:

| Parameter | Description |
|-----------|-------------|
| `isHttp` | When `true`, registers `HttpUserContext` (reads the user from `HttpContext`). Otherwise a `DefaultUserContext` (`"system"`) is used. |
| `assemblies` | The assemblies to scan for handlers/behaviors/validators. When omitted, the entry assembly is scanned. The Mediarq assembly itself is always scanned for the built-in behaviors. |

Inject `IMediator`, or the narrower `ISender` (commands/queries) / `IPublisher` (notifications).

### Reflection-free registration (source generator)

For startup with no assembly scan (trimming/AOT friendly), use `AddMediarqCore` together with the
compile-time generated `AddMediarqHandlers()` extension — shipped as an analyzer inside the package:

```csharp
builder.Services.AddMediarqCore(isHttp: false)
                .AddMediarqHandlers(); // generated at compile time
```

## Commands & queries (with a result)

```csharp
public record CreateUserCommand(string Name) : ICommand<Result<Guid>>;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        // ... persist ...
        return Task.FromResult(Result.Success(id));
    }
}

// Dispatch
Result<Guid> result = await mediator.Send(new CreateUserCommand("Alice"));
if (result.IsSuccess) { /* use result.Value */ }
```

Queries work the same way via `IQuery<TResponse>` / `IQueryHandler<TQuery, TResponse>`.

## No-result (void) commands

A command without a return value implements `ICommand` and is handled by `ICommandHandler<TCommand>`.
It flows through the **same pipeline** as any other request (its response type is `Unit`).

```csharp
public record DeleteUserCommand(Guid Id) : ICommand;

public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    public Task Handle(DeleteUserCommand request, CancellationToken cancellationToken = default)
    {
        // ... delete ...
        return Task.CompletedTask;
    }
}

await mediator.Send(new DeleteUserCommand(id));
```

## Notifications

A notification can be handled by zero or more handlers. All handlers are invoked when published.

```csharp
public record UserCreated(Guid Id) : INotification;

public class SendWelcomeEmail : INotificationHandler<UserCreated>
{
    public Task Handle(UserCreated notification, CancellationToken cancellationToken = default)
        => /* ... */ Task.CompletedTask;
}

await mediator.Publish(new UserCreated(id));
```

By default handlers run concurrently (`ParallelNotificationPublisher`) and the first failure is surfaced;
publishing with no registered handler is a no-op. Register a different `INotificationPublisher`
(e.g. `SequentialNotificationPublisher`, or your own) before `AddMediarq`/`AddMediarqCore` to change this.

## Pipeline behaviors

Cross-cutting logic wraps the handler. Mediarq ships with `LoggingBehavior`, `PerformanceBehavior`
and `ValidationBehavior`. Add your own by implementing `IPipelineBehavior<TRequest, TResponse>`:

```csharp
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        // before
        var response = await handle();
        // after
        return response;
    }
}
```

Behaviors discovered by the scan run in registration order. To control ordering, also implement
`IOrderBehavior` — behaviors with a **lower `Order` run first** (outermost):

```csharp
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    public int Order => 10;
    // ...
}
```

## Validation

Implement `IValidator<TRequest>`; the `ValidationBehavior` runs all validators before the handler
and short-circuits with a failed `Result` / `Result<T>` (carrying a `ValidationError`) when invalid:

```csharp
public class CreateUserCommandValidator : IValidator<CreateUserCommand>
{
    public IEnumerable<ValidationResult> Validate(CreateUserCommand instance)
    {
        if (string.IsNullOrWhiteSpace(instance.Name))
            yield return ValidationResult.Failure([new ValidationPropertyError(nameof(instance.Name), "Name is required.")]);
        else
            yield return ValidationResult.Success();
    }
}
```

## The `Result` type

`Result` / `Result<T>` express success or failure without exceptions:

```csharp
Result ok = Result.Success();
Result<int> value = Result.Success(42);
Result failed = Result.Failure(ResultError.NotFound("User.NotFound", "User not found"));

if (value.IsSuccess) Console.WriteLine(value.Value);
```

Compose them functionally, without manual `IsSuccess` checks (sync + async variants):

```csharp
string message =
    Result.Success(42)
        .Ensure(x => x > 0, ResultError.Failure("Id.Invalid", "must be positive"))
        .Map(x => x * 2)
        .Match(onSuccess: x => $"value: {x}", onFailure: e => $"error: {e.Message}");

// async, over Task<Result<T>>
Result<int> doubled = await GetResultAsync().MapAsync(x => x * 2);
```

## License

MIT © Nicolas Rouffart
