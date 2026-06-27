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

## Still stuck?

Compare your wiring against the runnable
[WebApi sample](https://github.com/rouffou/mediarq/tree/main/Samples/Mediarq.Samples.WebApi); it exercises
every extension end-to-end.
