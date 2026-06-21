# Migrating from MediatR

Mediarq covers the same building blocks as MediatR — requests, handlers, notifications and a behavior
pipeline — with a few deliberate differences: a railway-oriented `Result` type, a source generator for
reflection-free (Native-AOT friendly) dispatch, and a lean default pipeline.

## Concept mapping

| MediatR | Mediarq |
|---|---|
| `IRequest<TResponse>` | `ICommand<TResponse>` / `IQuery<TResponse>` (both are `ICommandOrQuery<TResponse>`) |
| `IRequest` (void) | `ICommand` (flows through the pipeline as `Unit`) |
| `IRequestHandler<TRequest, TResponse>` | `ICommandHandler<,>` / `IQueryHandler<,>` (both are `IRequestHandler<,>`) |
| `INotification` | `INotification` |
| `INotificationHandler<T>` | `INotificationHandler<T>` |
| `IPipelineBehavior<TRequest, TResponse>` | `IPipelineBehavior<TRequest, TResponse>` |
| `IStreamRequest<T>` / `IStreamRequestHandler<,>` | `IStreamRequest<T>` / `IStreamRequestHandler<,>` |
| `ISender` / `IPublisher` / `IMediator` | `ISender` / `IPublisher` / `IMediator` |
| `RequestHandlerDelegate<T> next` | `Func<Task<TResponse>> handle` |
| `AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))` | `AddMediarq(isHttp, assemblies)` (scan) **or** `AddMediarqCore()` + generated `AddMediarqHandlers()` (AOT) |

## Registration

MediatR:

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

Mediarq — runtime scan (simplest):

```csharp
services.AddMediarq(isHttp: false, typeof(Program).Assembly);
```

Mediarq — reflection-free / Native AOT (recommended for trimming/AOT):

```csharp
services.AddMediarqCore()
        .AddMediarqHandlers(); // generated at compile time by the source generator
```

## Handlers

MediatR:

```csharp
public record Ping(string Message) : IRequest<string>;

public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken ct) => Task.FromResult(request.Message);
}
```

Mediarq (idiomatic — return a `Result<T>`):

```csharp
public record Ping(string Message) : IQuery<Result<string>>;

public sealed class PingHandler : IQueryHandler<Ping, Result<string>>
{
    public Task<Result<string>> Handle(Ping request, CancellationToken ct = default)
        => Task.FromResult(Result.Success(request.Message));
}
```

> Mediarq does not force `Result`. `IQuery<string>` with a `string`-returning handler works too — but
> `Result`/`Result<T>` is the idiomatic way to model success/failure without exceptions, and it is what
> `ValidationBehavior` and the ASP.NET Core mapping build on.

## Behaviors

The behavior interface is nearly identical; the `next` delegate is a `Func<Task<TResponse>>`:

```csharp
public sealed class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
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

See [Writing a behavior](writing-a-behavior.md) for ordering and conditional behaviors.

## Notifications

`Publish` works the same. Two differences worth knowing:

- The publishing strategy is configurable via `INotificationPublisher`
  (`ParallelNotificationPublisher` by default, `SequentialNotificationPublisher`, or your own).
- An `IValidator<TNotification>` runs automatically before publishing; an invalid notification throws a
  `NotificationValidationException`.

## Behavior differences to be aware of

- **Lean by default.** Unlike registering a behavior for every request, Mediarq's built-in
  validation / pre / post / exception behaviors are registered only when a matching
  validator / processor / exception handler exists, and logging/performance are opt-in
  (`AddMediarqRequestLogging()` / `AddMediarqPerformanceTracking()`). A request with no applicable
  behavior dispatches straight to the handler.
- **`Result` instead of exceptions** for expected failures (validation, not-found, conflicts).
- **Source generator.** For trimming/Native AOT, prefer `AddMediarqCore()` + `AddMediarqHandlers()`.
  See [Native AOT](native-aot.md).
