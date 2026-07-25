# Wiring — registering the core and each extension

A cheat sheet for getting every piece registered correctly the first time. Each section lists the
package, the one registration call, the marker interface (if any), a minimal example, and the
**prerequisite / gotcha** that usually trips people up.

## Registering the core

Two ways — pick one:

```csharp
// A. Assembly scan — convenient, uses reflection.
builder.Services.AddMediarq(isHttp: true, typeof(Program).Assembly);

// B. Reflection-free / Native AOT — source-generated, no scan.
builder.Services.AddMediarqCore(isHttp: true).AddMediarqHandlers();
```

- `isHttp: true` reads the current user from `HttpContext` → **you must also call**
  `builder.Services.AddHttpContextAccessor();`. Use `isHttp: false` for a console/worker.
- With **A (scan)**, the built-in `ValidationBehavior` / pre / post / exception behaviors are wired
  **only if** a matching validator / processor / handler is discovered. ⚠️ **Order matters** — see the
  validation gotcha below and [Troubleshooting](troubleshooting.md). With **B**, all four built-ins are
  always registered, so order doesn't matter.

Opt-in core behaviors (any order, after the call above):

```csharp
.AddMediarqRequestLogging()      // logs each request   (active only when Information logging is enabled)
.AddMediarqPerformanceTracking() // warns on slow ones  (active only when Warning logging is enabled)
.AddMediarqTimeout()             // enforces ITimeoutRequest.Timeout -> RequestTimeoutException
```

## Validation

### Mediarq.FluentValidation
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<Program>(); // your AbstractValidator<T> classes
builder.Services.AddMediarqFluentValidation();
```
⚠️ With the **scan** core, call `AddMediarqFluentValidation()` **before** `AddMediarq(...)`, otherwise
the scan sees no `IValidator<>` and the validation behavior is never wired (validation silently does
nothing). Not an issue with `AddMediarqCore()`.

### Mediarq.DataAnnotations
```csharp
builder.Services.AddMediarqDataAnnotations(); // validates every request against its [Required]/[Range]/… attributes
```
Same ordering note as above. Requests without attributes simply pass.

> You can use built-in `IValidator<T>` classes, FluentValidation and DataAnnotations together — every
> matching validator runs.

## Mediarq.AspNetCore — Result → HTTP

No registration needed; just call the extension methods on a `Result` in your endpoint/controller:

```csharp
group.MapPost("/", (CreateOrder cmd, ISender s) => s.Send(cmd).ToHttpResultAsync()); // minimal API
// MVC: return (await mediator.Send(cmd)).ToActionResult();
```
Success → `200`/`Ok(value)`; failure → RFC 7807 `ProblemDetails` with a status derived from
`ResultError.Type` (`NotFound` → 404, `Validation` → 400, `Conflict` → 409, …).

## Mediarq.Caching — memoize a query

```csharp
builder.Services.AddMediarqCaching();            // in-memory (calls AddMemoryCache() for you)
// or, distributed:
builder.Services.AddDistributedMemoryCache();    // or AddStackExchangeRedisCache(...)
builder.Services.AddMediarqDistributedCaching();
```
Marker: `ICacheableRequest` (`CacheKey`, optional `CacheDuration`). Call **after** the core.

```csharp
public record GetOrder(Guid Id) : IQuery<Result<OrderDto>>, ICacheableRequest
{
    public string CacheKey => $"orders:{Id}";
    public TimeSpan? CacheDuration => TimeSpan.FromSeconds(30);
}
```
⚠️ Distributed caching serializes the response to JSON — keep your DTOs serializable. `Result` / `Result<T>`
round-trip out of the box.

## Mediarq.Idempotency — run once per key

```csharp
builder.Services.AddDistributedMemoryCache(); // REQUIRED (IDistributedCache); use Redis in production
builder.Services.AddMediarqIdempotency();
```
Marker: `IIdempotentRequest` (`IdempotencyKey`, optional `IdempotencyDuration`). A repeated request with
the same key replays the stored result instead of re-running the handler. Use a stable, caller-supplied
key (e.g. an `Idempotency-Key` HTTP header).

## Mediarq.UnitOfWork & Mediarq.EntityFrameworkCore — transactional commands

```csharp
builder.Services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("app")); // your context first
builder.Services.AddMediarqEntityFrameworkCore<AppDbContext>(); // DbContext as the unit of work
```
Marker: `ITransactionalRequest`. The unit-of-work behavior calls `SaveChangesAsync()` **after** the
handler, and **only when the result is not a failure** — so a handler just stages changes:

```csharp
public record CreateOrder(string Customer) : ICommand<Result<Guid>>, ITransactionalRequest;
// handler: db.Orders.Add(order);  return Result.Success(order.Id);   // no SaveChanges() needed
```
(For a non-EF store, implement `IUnitOfWork` yourself and call `AddMediarqUnitOfWork()`.)

## Mediarq.Outbox — reliable events

```csharp
builder.Services.AddMediarqOutbox<AppDbContext>(o => o.PollingInterval = TimeSpan.FromSeconds(5));
```
Plus, in your context: `modelBuilder.ApplyMediarqOutbox();` inside `OnModelCreating`. In a handler,
stage an event with `IOutbox.Enqueue(...)`; it is committed in the same transaction as your data and
published afterwards by the background `OutboxProcessor`. Combine with the unit of work above so the
event and the data commit atomically.

## Mediarq.Saga — process managers

```csharp
builder.Services.AddMediarqSaga<OrderFulfillmentState>();
```
State: `ISagaState` (`CorrelationId`, `IsComplete`). Each step is a normal `INotificationHandler`
implemented by deriving from `SagaNotificationHandler<TNotification, TState>` — it loads (or creates) the
`TState` correlated to the notification via an `ISagaStore<TState>`, calls your `HandleAsync`, then saves
it back. Steps that share the same `CorrelationId` read and write the same instance across separate
dispatches. The default store is process-lifetime only; register your own `ISagaStore<TState>` (e.g. EF
Core-backed) **before** `AddMediarqSaga<TState>()` for production. Pair with `Mediarq.Outbox`'s
`IOutbox.Enqueue` inside `HandleAsync` so a step's follow-up event is delivered reliably.

## Mediarq.Hangfire — delayed & scheduled dispatch

```csharp
builder.Services.AddMediarqHangfire();
builder.Services.AddHangfire(cfg => cfg.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();
```
```csharp
backgroundJobClient.Enqueue(new SendWelcomeEmail(userId));                                    // ASAP
backgroundJobClient.Schedule(new SendWelcomeEmail(userId), TimeSpan.FromMinutes(10));          // after a delay
backgroundJobClient.Schedule(new SendWelcomeEmail(userId), DateTimeOffset.UtcNow.AddDays(1));  // at a point in time
```
Only `ICommand` (no result) is supported — call `Enqueue`/`Schedule` directly on the command instance,
not through a variable statically typed as `ICommand`, so Hangfire's serializer captures the concrete
type.

## Mediarq.Quartz — delayed & scheduled dispatch (Quartz.NET)

```csharp
builder.Services.AddMediarqQuartz();
builder.Services.AddQuartz();
builder.Services.AddQuartzHostedService();
```
```csharp
await scheduler.EnqueueAsync(new SendWelcomeEmail(userId));                                    // ASAP
await scheduler.ScheduleAsync(new SendWelcomeEmail(userId), TimeSpan.FromMinutes(10));          // after a delay
await scheduler.ScheduleAsync(new SendWelcomeEmail(userId), DateTimeOffset.UtcNow.AddDays(1));  // at a point in time
```
Same `ICommand`-only constraint as `Mediarq.Hangfire`. The command is JSON-serialized into the job's
`JobDataMap`; configure a persistent Quartz job store for jobs to survive a restart.

## Mediarq.HealthChecks — fail fast on a missing/ambiguous handler

```csharp
builder.Services.AddHealthChecks()
    .AddMediarqHandlerRegistrationCheck(assemblies: typeof(Program).Assembly);
```
```csharp
app.MapHealthChecks("/health");
```
Reports `Unhealthy` when a discovered `ICommand`/`IQuery` closed type does not resolve to exactly one
`IRequestHandler<TRequest, TResponse>`. To fail the app at startup instead of waiting for a health probe:

```csharp
builder.Services.AddMediarqHandlerValidationOnStartup(typeof(Program).Assembly);
```
Throws `InvalidOperationException` once, during host startup, if any command/query has zero or more than
one registered handler.

## Mediarq.Authorization — policy-based authorization

```csharp
builder.Services.AddAuthorization();          // ASP.NET Core's own registration
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediarqAuthorization();
```
Marker: `IAuthorizedRequest` (`PolicyName`). No authenticated user → `Result.Failure(ResultError.Unauthorized(...))`
(HTTP 401 via `Mediarq.AspNetCore`); authenticated but the policy fails → `ResultError.Forbidden(...)`
(HTTP 403). `PolicyName` can be `null` to only require authentication.

```csharp
public record DeleteOrder(Guid OrderId) : ICommand, IAuthorizedRequest
{
    public string? PolicyName => "OrdersAdmin";
}
```
⚠️ The handler's response type must be `Result` or `Result<T>` — same constraint, same reflection-fallback
trade-off, as the core `ValidationBehavior`'s `Result<T>` support.

## Mediarq.Polly — resilience

```csharp
builder.Services.AddResiliencePipeline("orders-pricing", p => p.AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 }));
builder.Services.AddMediarqResilience();
```
Marker: `IResilientRequest` (`ResiliencePipelineName`). ⚠️ The name returned by the request **must match**
a pipeline registered via `AddResiliencePipeline(name, …)`, or resolution throws at dispatch.

## Mediarq.Diagnostics & Mediarq.OpenTelemetry — tracing & metrics

```csharp
builder.Services.AddMediarqDiagnostics(); // ⚠️ AFTER AddMediarq/AddMediarqCore (it decorates the publisher)
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddMediarqInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddMediarqInstrumentation().AddOtlpExporter());
```
`AddMediarqInstrumentation()` subscribes to the `"Mediarq"` source/meter.

## Mediarq.MassTransit — out-of-process notifications

```csharp
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    x.UsingInMemory((ctx, cfg) => cfg.ConfigureEndpoints(ctx)); // or RabbitMQ/Azure SB
});
builder.Services.AddMediarqMassTransitForwarding<OrderPlacedEvent>(); // forward this event onto the bus
```
Marker: `IIntegrationEvent` (an `INotification` meant to leave the process). The forwarder is a normal
notification handler, so it runs **alongside** your in-process handlers.

## Recommended order (a safe template)

```csharp
// 1. infrastructure the extensions need
builder.Services.AddDbContext<AppDbContext>(...);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();              // if isHttp: true
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 2. validation adapters BEFORE the (scan) core
builder.Services.AddMediarqFluentValidation();
builder.Services.AddMediarqDataAnnotations();

// 3. the core
builder.Services.AddMediarq(isHttp: true, typeof(Program).Assembly)
                .AddMediarqRequestLogging();

// 4. everything else AFTER the core
builder.Services.AddMediarqEntityFrameworkCore<AppDbContext>();
builder.Services.AddMediarqOutbox<AppDbContext>();
builder.Services.AddMediarqSaga<OrderFulfillmentState>();
builder.Services.AddMediarqCaching();
builder.Services.AddMediarqIdempotency();
builder.Services.AddMediarqResilience();
builder.Services.AddMediarqDiagnostics();
builder.Services.AddMediarqAuthorization();
builder.Services.AddMediarqHandlerValidationOnStartup(typeof(Program).Assembly);
```

See [Samples/Mediarq.Samples.WebApi](https://github.com/rouffou/mediarq/tree/main/Samples/Mediarq.Samples.WebApi)
for this exact wiring in a runnable app.
