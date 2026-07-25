# Troubleshooting & FAQ

Common symptoms, the cause, and the fix. Most "nothing happens" issues come down to registration.

## Dispatch

### `HandlerNotFoundException` when I call `Send`
No handler is registered for that request type.
- **Scan core** (`AddMediarq`): make sure the handler lives in an assembly you passed to `AddMediarq(...)`.
  Handlers in another project are not found unless you pass that assembly too.
- **Generated core** (`AddMediarqCore`): make sure you also called `.AddMediarqHandlers()` and that the
  request and handler are in the **same project** the source generator runs in.
- Check the handler implements the right interface for the request: `ICommandHandler<TReq, TРes>` for an
  `ICommand<TРes>`, `IQueryHandler<,>` for an `IQuery<>`, etc.

### Build warning/error `MQ001` — multiple handlers for one request
A command/query must have exactly one handler. You have two classes implementing the handler interface
for the same request. Delete or merge one.

### Build info `MQ002` — a command/query has no handler in the assembly
You declared an `ICommand<>`/`IQuery<>` but never wrote its handler (or it's in another assembly the
generator doesn't see). Add the handler, or ignore if intentional.

### Build info `MQ003` — a validator targets neither a request nor a notification
Your `IValidator<T>` is for a `T` that is never dispatched, so it can never run. Point it at a request
or notification type.

## Lifetime

### Build warning `MQ200` — Singleton depends on a shorter-lived type
A handler, behavior or validator marked `[RegisterHandler(ServiceLifetime.Singleton)]` constructor-injects
a `DbContext`, or another Mediarq type explicitly marked `Scoped`/`Transient`. That dependency is resolved
**once** and captured for the app's whole lifetime instead of once per scope/request — the classic
*captive dependency* bug (a `DbContext` shared and mutated concurrently across requests, or stale data
from a scoped service that should have been refreshed).

Fix by either:
- Lowering the Singleton's lifetime to `Scoped` (the default — just remove the attribute, or pass
  `ServiceLifetime.Scoped`), if it doesn't actually need to be a Singleton; or
- Injecting `IServiceScopeFactory` / `IServiceProvider` instead and resolving the scoped dependency inside
  a short-lived scope created per use, instead of depending on it directly.

This analyzer only compares **explicit** lifetimes (`[RegisterHandler(...)]` on both sides, or a
`DbContext`-derived parameter type) — it has no visibility into a consumer's arbitrary
`services.AddScoped<T>()` calls outside the Mediarq registration model, so it stays silent rather than
guess and risk a false positive.

## Pipeline

### Build warning `MQ201` — pipeline behavior never calls its `handle` delegate
An `IPipelineBehavior<TRequest, TResponse>.Handle` implementation never references its `handle`
parameter anywhere in its body. Every behavior after this one in the pipeline — and the actual request
handler — will never run; the behavior always short-circuits with whatever it returns directly.

```csharp
public Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
{
    // Missing `return await handle();` — the request never reaches its handler.
    return Task.FromResult(default(TResponse)!);
}
```

Fix by calling `handle()` (typically `return await handle();`, or storing its result to act on
afterwards). If the short-circuit is genuinely intentional (e.g. a behavior that always returns a
canned response and never delegates further), the analyzer has no way to tell that apart from a bug —
suppress the specific instance rather than the rule.

This analyzer only requires `handle` to appear **somewhere** in the body, on any code path — a behavior
that conditionally short-circuits (e.g. return a cached value on a hit, otherwise `await handle()`) is
not flagged, since the identifier is still referenced on the miss path.

### Build warning `MQ204` — pipeline behavior is registered but never active
An `IConditionalPipelineBehavior.IsActive` implementation is literally `false` — the behavior is
registered but can never participate in the pipeline for any request:

```csharp
public bool IsActive => false; // leftover placeholder, or a mistake -- this behavior never runs
```

Fix by implementing the real activation condition, or remove the behavior/its registration if it's
genuinely dead code.

This is a syntactic check, not a proof over every possible request type: it only fires when the getter
is literally the `false` literal (an expression-bodied property, an expression-bodied getter, or a single
`return false;`) — real conditional logic, however it evaluates at runtime, is never flagged.

## Validation

### My validation never runs (no error, the handler just runs)
The most common footgun, and **only on the scan core** (`AddMediarq`). The built-in `ValidationBehavior`
is wired only if a validator is visible when `AddMediarq(...)` runs.
- If your validators come from **FluentValidation / DataAnnotations adapters**, call
  `AddMediarqFluentValidation()` / `AddMediarqDataAnnotations()` **before** `AddMediarq(...)`.
- If you write native `IValidator<T>` classes, make sure they're in a scanned assembly.
- Or switch to `AddMediarqCore().AddMediarqHandlers()`, where the behavior is always registered.

See [Wiring extensions](wiring-extensions.md#validation).

### FluentValidation validator isn't picked up
Register your `AbstractValidator<T>` classes with FluentValidation's own DI helper —
`services.AddValidatorsFromAssemblyContaining<Program>()` — in addition to `AddMediarqFluentValidation()`.

### A notification with a validator throws instead of returning a failed Result
That's by design: a notification has no return value, so an invalid one throws
`NotificationValidationException` (carrying the property errors). Catch it where you publish, or validate
before publishing.

## Behaviors

### My custom behavior doesn't run
- Scan core: the behavior must be an `IPipelineBehavior<,>` in a scanned assembly. Generated core: it's
  picked up by `AddMediarqHandlers()`. You can also register it explicitly:
  `services.AddScoped(typeof(IPipelineBehavior<,>), typeof(MyBehavior<,>));`
- If it implements `IConditionalPipelineBehavior`, check that `IsActive` returns `true` for the request.

### My behaviors run in the wrong order
Behaviors run in registration order unless they implement `IOrderBehavior` — **lower `Order` runs first
(outermost)**. The exception behavior is outermost, the handler is innermost. See
[Writing a behavior](writing-a-behavior.md#ordering--iorderbehavior).

## Extensions

### Caching/idempotency: `Result` (or my DTO) fails to serialize
Distributed caching and idempotency serialize the response to JSON. `Result` / `Result<T>` round-trip
out of the box; for your own payloads, make sure the value type is serializable (a record/POCO with a
public constructor). On Native AOT, register a source-generated `IMediarqCacheSerializer`.

### Idempotency does nothing / throws about `IDistributedCache`
You must register an `IDistributedCache` — `AddDistributedMemoryCache()` for a single process, or Redis.

### Diagnostics produces no traces/metrics
`AddMediarqDiagnostics()` decorates the notification publisher, so call it **after**
`AddMediarq`/`AddMediarqCore`. Subscribe with OpenTelemetry via `AddMediarqInstrumentation()` (it adds
the `"Mediarq"` source/meter) and an exporter.

### Polly: pipeline name not found
The `ResiliencePipelineName` on your `IResilientRequest` must match a pipeline registered with
`AddResiliencePipeline("that-name", …)`. Also call `AddMediarqResilience()`.

### `isHttp: true` but the user is always "system" / a null-ref on the HTTP context
Register `AddHttpContextAccessor()`. Without it, `HttpUserContext` has no context to read.

### A transactional command didn't persist
`ITransactionalRequest` commits via the unit of work **only when the result is not a failure**. If your
handler returns a failed `Result`, nothing is saved — by design. Also confirm
`AddMediarqEntityFrameworkCore<TContext>()` (or `AddMediarqUnitOfWork()`) is registered, and that your
handler stages changes on the **same** `DbContext` instance it's injected with.

### Outbox events are never published
Check all three: `modelBuilder.ApplyMediarqOutbox()` in `OnModelCreating`, `AddMediarqOutbox<TContext>()`
registered, and the unit of work commits (the `IOutbox.Enqueue` row is saved with your data). The
background processor then publishes within its `PollingInterval`.

## Native AOT

### Trim/AOT warnings, or it fails to publish
Use the reflection-free path: `AddMediarqCore().AddMediarqHandlers()` (not the `AddMediarq` scan, which is
annotated `[RequiresUnreferencedCode]`). For caching/idempotency on AOT, provide a source-generated
`IMediarqCacheSerializer`. A native publish also needs the platform C/C++ build tools on `PATH`. See
[Native AOT & trimming](native-aot.md).

### Build warning `MQ004` — reflection-based `AddMediarq` used in an AOT-published project
Your project sets `PublishAot`/`IsAotCompatible`, but a call site still uses the reflection-based
`AddMediarq(...)` scan. Switch it to `AddMediarqCore().AddMediarqHandlers()` — see
[Native AOT & trimming](native-aot.md).

## Still stuck?

Compare your wiring against the runnable
[WebApi sample](https://github.com/rouffou/mediarq/tree/main/Samples/Mediarq.Samples.WebApi); it exercises
every extension end-to-end.
