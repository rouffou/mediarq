# Mediarq

[![CI](https://github.com/rouffou/mediarq/actions/workflows/ci.yml/badge.svg)](https://github.com/rouffou/mediarq/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/rouffou/mediarq/branch/main/graph/badge.svg)](https://codecov.io/gh/rouffou/mediarq)
[![NuGet](https://img.shields.io/nuget/v/Mediarq.svg)](https://www.nuget.org/packages/Mediarq)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-8%20%7C%209%20%7C%2010-512BD4.svg)](https://dotnet.microsoft.com/)

A lightweight, dependency-free **CQRS mediator for .NET** — a free alternative to MediatR with
commands, queries, no-result commands, notifications, a composable pipeline of behaviors, and
built-in validation and `Result` types. Designed for domain-driven and CQRS architectures.

- ✅ Commands / queries returning a `Result` (railway-oriented)
- ✅ No-result (void) commands routed through the same pipeline
- ✅ Notifications published to multiple handlers
- ✅ Streaming requests (`IStreamRequest<T>` → `IAsyncEnumerable<T>`)
- ✅ Pipeline behaviors (logging, performance, validation, exception handling) + your own, orderable
- ✅ Built-in validation abstraction and `Result` / `ResultError` types
- ✅ **Reflection-free dispatch** via an optional source generator — **trimming/Native AOT friendly**
- ✅ Functional `Result` combinators (`Map`, `Bind`, `Match`, `Tap`, `Ensure`)

> Targets **.NET 8, .NET 9 and .NET 10**.

---

## Installation

```bash
dotnet add package Mediarq        # lean meta-package: core + lightweight extensions
# or, for the core only:
dotnet add package Mediarq.Core   # mediator, pipeline, results, source generator
```

The **`Mediarq`** meta-package bundles `Mediarq.Core` with the lightweight extensions (ASP.NET Core,
FluentValidation, DataAnnotations, Caching, Diagnostics, UnitOfWork). Heavy or opinionated
integrations — `Mediarq.EntityFrameworkCore`, `Mediarq.OpenTelemetry`, `Mediarq.MassTransit` and
`Mediarq.Polly` — ship separately and are installed explicitly when needed (see
[Extension packages](#extension-packages)). Reference `Mediarq.Core` plus only the extensions you
need to keep dependencies minimal.

### Scaffolding (`dotnet new`)

Install the templates and scaffold a feature (a command, its handler and a validator) in one command:

```bash
dotnet new install Mediarq.Templates
dotnet new mediarq-feature -n CreateUser --namespace MyApp.Users
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

### Reflection-free registration (source generator) — Native AOT

For startup with no assembly scan, use `AddMediarqCore` together with the compile-time generated
`AddMediarqHandlers()` extension — shipped as an analyzer inside the package:

```csharp
builder.Services.AddMediarqCore(isHttp: false)
                .AddMediarqHandlers(); // generated at compile time
```

On this path there is **no reflection at all** on dispatch: the generator pre-populates a registry of
strongly-typed `Send`/notification wrappers (no `Activator.CreateInstance`, no `MakeGenericType`), and
the validation pipeline builds `Result<T>` failures from generated factories. The library is marked
`IsAotCompatible` and publishes cleanly with **Native AOT** (see [Samples/Mediarq.AotSample](Samples/Mediarq.AotSample)).

> The scan-based `AddMediarq(...)` is convenient but uses reflection and is annotated
> `[RequiresUnreferencedCode]`; prefer `AddMediarqCore()` + `AddMediarqHandlers()` for trimming/AOT.

The generated `AddMediarqHandlers()` is `internal` and lives in the `Mediarq.Extensions` namespace by
default. Override either via MSBuild:

```xml
<PropertyGroup>
  <MediarqGeneratedAccessibility>public</MediarqGeneratedAccessibility>
  <MediarqGeneratedNamespace>MyApp.Generated</MediarqGeneratedNamespace>
</PropertyGroup>
```

The generator also emits compile-time diagnostics: `MQ001` (multiple handlers for one request), `MQ002`
(a command/query with no handler in the assembly), and `MQ003` (a validator whose target is neither a
request nor a notification, so it can never run).

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

### Out-of-process notifications (MassTransit)

The optional **`Mediarq.MassTransit`** package forwards notifications to a MassTransit bus, so other
services can consume them out-of-process. The forwarder is a regular notification handler, so it runs
alongside your in-process handlers.

```csharp
// Configure MassTransit as usual (provides IPublishEndpoint), then:
builder.Services.AddMediarqMassTransitForwarding<OrderPlaced>();          // one event, or
builder.Services.AddMediarqMassTransitForwarding(typeof(OrderPlaced).Assembly); // every IIntegrationEvent

public record OrderPlaced(Guid Id) : IIntegrationEvent; // IIntegrationEvent : INotification
```

`await mediator.Publish(new OrderPlaced(id))` now runs the local handlers **and** publishes the event on
the bus.

## Pipeline behaviors

Cross-cutting logic wraps the handler. The pipeline is **lean by default**: the built-in
`ValidationBehavior`, pre/post-processor and exception behaviors register only when you actually have a
validator / processor / exception handler, so an idle request resolves no behavior at all. Request
logging and performance tracking are **opt-in**:

```csharp
services.AddMediarq(/* ... */)
        .AddMediarqRequestLogging()       // LoggingBehavior
        .AddMediarqPerformanceTracking()  // PerformanceBehavior
        .AddMediarqTimeout();             // TimeoutBehavior
```

`AddMediarqTimeout()` bounds requests that implement `ITimeoutRequest`: if handling exceeds the
request's `Timeout`, a `RequestTimeoutException` is thrown (a pessimistic timeout — it frees the caller,
so handlers should also honor their `CancellationToken`). It is inert for other request types.

Add your own by implementing `IPipelineBehavior<TRequest, TResponse>`:

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

A behavior can also opt out of the pipeline per request type by implementing
`IConditionalPipelineBehavior` and returning `IsActive => false` — the executor then omits it entirely,
adding neither an async frame nor a delegate. The built-in behaviors use this so an idle pipeline costs
nothing: validation/pre/post/exception activate only when a validator, processor or exception handler is
registered, and logging/performance only when the matching log level is enabled. As a result, dispatch
for a request with no active behavior goes straight to the handler — allocations are on par with MediatR.

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

Notifications are validated too: define an `IValidator<TNotification>` and it runs automatically before
the notification is published. Because a notification has no return value, an invalid one throws a
`NotificationValidationException` (carrying the property errors) instead of returning a failed `Result`.

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

## Extension packages

Mediarq ships optional, opt-in packages so the core stays dependency-free:

| Package | Purpose |
|---|---|
| `Mediarq.AspNetCore` | Map `Result` / `ResultError` → `IResult` and RFC 7807 ProblemDetails |
| `Mediarq.FluentValidation` | Run FluentValidation validators in the Mediarq pipeline |
| `Mediarq.DataAnnotations` | Validate requests with `System.ComponentModel.DataAnnotations` attributes (`AddMediarqDataAnnotations`) |
| `Mediarq.Caching` | Memoize responses of `ICacheableRequest` via `IMemoryCache` (`AddMediarqCaching`) or `IDistributedCache` / Redis (`AddMediarqDistributedCaching`) |
| `Mediarq.Idempotency` | Run `IIdempotentRequest` at most once per key, replaying the stored result (`AddMediarqIdempotency`) |
| `Mediarq.Outbox` | Transactional outbox over EF Core: enqueue notifications and publish them reliably (`AddMediarqOutbox`) |
| `Mediarq.Diagnostics` | `Activity` tracing + metrics (OpenTelemetry-compatible) (`AddMediarqDiagnostics`) |
| `Mediarq.OpenTelemetry` | One-line `AddMediarqInstrumentation()` on the tracer/meter provider builders |
| `Mediarq.UnitOfWork` | Commit a unit of work around `ITransactionalRequest` commands (`AddMediarqUnitOfWork`) |
| `Mediarq.EntityFrameworkCore` | `EfCoreUnitOfWork<TContext>` over a `DbContext` (`AddMediarqEntityFrameworkCore`) |
| `Mediarq.Polly` | Retry / timeout / circuit breaker for `IResilientRequest` via Polly (`AddMediarqResilience`) |
| `Mediarq.MassTransit` | Forward notifications to a MassTransit bus, out-of-process (`AddMediarqMassTransitForwarding`) |

Built into `Mediarq.Core`:

- **Exception handling** — implement `IRequestExceptionHandler<TRequest, TResponse>` to turn an exception into a response (typically a failed `Result`).
- **Pre/post processors** — `IRequestPreProcessor<TRequest>` and `IRequestPostProcessor<TRequest, TResponse>` run around the handler.
- **Streaming pipeline** — `IStreamPipelineBehavior<TRequest, TResponse>` wraps `CreateStream` with the same ordering as `Send` behaviors.
- **Ordered notifications** — a notification handler can implement `IOrderedNotificationHandler` for a deterministic order.
- **Lifetime control** — opt a handler into a DI lifetime with `[RegisterHandler(ServiceLifetime.Singleton)]`.
- **Validation localization** — translate messages via `IValidationMessageResolver`.
- **More `Result` combinators** — `Combine`, `Try`/`TryAsync`, `TryGetValue`, `Recover`, `OrElse`, `ToResult`, plus cross async `MapAsync`/`BindAsync` overloads.
- **`AggregateExceptionNotificationPublisher`** — runs every notification handler and surfaces *all* failures.

## Samples

Three runnable samples under [Samples/](Samples) (see [Samples/README.md](Samples/README.md)):

- **[Mediarq.Samples.Quickstart](Samples/Mediarq.Samples.Quickstart)** — a console tour of the core
  in-process features (commands/queries/void, notifications, streaming, validation, behaviors,
  pre/post processors, exception handling, timeout, `Result` combinators).
- **[Mediarq.Samples.WebApi](Samples/Mediarq.Samples.WebApi)** — an ASP.NET Core "Orders" API wiring the
  extensions end-to-end (`Result` → HTTP, FluentValidation/DataAnnotations, caching, idempotency,
  EF Core unit of work + transactional outbox, Polly, diagnostics/OpenTelemetry, MassTransit).
- **[Mediarq.AotSample](Samples/Mediarq.AotSample)** — the reflection-free path, published with Native AOT.

```bash
dotnet run --project Samples/Mediarq.Samples.Quickstart
dotnet run --project Samples/Mediarq.Samples.WebApi      # then open /scalar/v1
dotnet run --project Samples/Mediarq.AotSample
```

## Documentation

Task-focused guides live under [docs/guides](docs/guides) (and on the docs site). If you're starting
from scratch with no one to ask, read them in this order:

1. [Concepts](docs/guides/concepts.md) — commands vs queries vs notifications, `Result`, the pipeline.
2. [Your first app](docs/guides/your-first-app.md) — build a working API step by step.
3. [Wiring extensions](docs/guides/wiring-extensions.md) — register the core and each optional package (with the prerequisites and gotchas).
4. [Writing a behavior](docs/guides/writing-a-behavior.md) · [Testing](docs/guides/testing.md) · [Migrating from MediatR](docs/guides/migrating-from-mediatr.md) · [Native AOT & trimming](docs/guides/native-aot.md).
5. [Troubleshooting](docs/guides/troubleshooting.md) — when something silently doesn't fire (start here when stuck).

## License

MIT © Nicolas Rouffart
