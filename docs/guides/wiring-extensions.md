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

## Cascaded notifications — `Result.WithNotifications(...)`

No registration needed. A handler can attach follow-up notifications to its own successful result
instead of injecting `IPublisher` and calling `Publish(...)` itself:

```csharp
public sealed class CreateOrderHandler : ICommandHandler<CreateOrder, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        // ... persist the order ...
        return Task.FromResult(Result.Success(id).WithNotifications(new OrderPlaced(id)));
    }
}
```
The mediator publishes `OrderPlaced` automatically once the request has finished dispatching — after
every behavior, exception handler and post-processor has run — through the same `IPublisher` (and
therefore the same registered `INotificationPublisher`: Parallel/Sequential/AggregateException) as an
explicit `Publish(...)` call. **Only on a successful result**: a failed `Result`/`Result<T>` never
publishes its attached notifications, even if `WithNotifications(...)` was called before the failure was
known. Works with both `Result` and `Result<T>`; has no effect on other response shapes (e.g. `Unit`).

Not automatically enqueued to `Mediarq.Outbox` — combine with an explicit `IOutbox.Enqueue(...)` call
inside the handler if a cascaded notification needs the outbox's transactional/reliable delivery
guarantee instead of an in-process publish.

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

Skip the pass-through endpoint entirely — annotate the command/query and map every attributed type at once:
```csharp
[MediarqGet("/orders/{id}")]
public record GetOrder(Guid Id) : IQuery<Result<OrderDto>>;

app.MapMediarq(typeof(GetOrder).Assembly);
```
`[MediarqGet]`/`[MediarqDelete]` bind members individually from the route/query string (`[AsParameters]`,
no body); `[MediarqPost]`/`[MediarqPut]`/`[MediarqPatch]` bind the whole request from the JSON body.

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

Domain events — publish events raised by aggregates after a successful commit:
```csharp
builder.Services.AddMediarqDomainEvents(); // picked up automatically by AddDbContext<AppDbContext>
```
```csharp
public class Order : AggregateRoot // or implement IHasDomainEvents directly
{
    public void Place() => AddDomainEvent(new OrderPlaced(Id));
}
```
Collected (and cleared) right before `SaveChangesAsync`, published only after it succeeds. Async-only —
no synchronous `SaveChanges` support.

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

## Mediarq.Deferred — in-process deferred dispatch, no external dependency

```csharp
builder.Services.AddMediarqDeferredDispatch();
```
```csharp
await deferredDispatcher.SendLaterAsync(new SendWelcomeEmail(userId));   // returns immediately
await deferredDispatcher.PublishLaterAsync(new UserRegistered(userId));
```
`IDeferredDispatcher` queues the request on a `System.Threading.Channels`-backed background worker
(`DeferredDispatchHostedService`) instead of running its handler(s) inline. Unlike `Mediarq.Hangfire`/
`Mediarq.Quartz`, the queue is **in-memory only** — no delay/cron scheduling, nothing survives a crash —
but a graceful shutdown drains everything already queued before the host stops. Use it for "reliable
in-process fire-and-forget" (e.g. decoupling an HTTP request from a side effect); use Hangfire/Quartz when
you need delayed/cron scheduling or durability across a restart.

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

## Mediarq.Testing — spy mediator, fakes

```csharp
services.AddMediarq(isHttp: false, typeof(Program).Assembly); // your normal registration
services.AddMediarqSpy();                                     // decorates IMediator, after AddMediarq(...)
```
```csharp
var spy = (SpyMediator)provider.GetRequiredService<IMediator>();
await provider.GetRequiredService<ISender>().Send(new CreateOrder(customerId));

spy.HasSent<CreateOrder>();          // true — recorded, and the real handler ran
spy.Published<OrderCreated>();       // notifications published during that Send
```
Handlers, validators and behaviors all run for real — nothing is faked, only recorded. Also ships
`FakeClock`/`FakeUserContext`, plain settable implementations of `IClock`/`IUserContext` to register in a
test's `IServiceCollection`.

## Mediarq.RateLimiting — throttle a request type or a user

```csharp
builder.Services.AddMediarqRateLimiting(registry =>
{
    registry.AddPolicy("orders-per-user", key => RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
    {
        Window = TimeSpan.FromMinutes(1),
        PermitLimit = 10,
    }));
});
```
Marker: `IRateLimitedRequest` (`PolicyName`, optional `PartitionKey` — e.g. the current user id, for an
independent limit per caller; `null` shares a single bucket). No permit available →
`RateLimitExceededException`; catch it with an `IRequestExceptionHandler<,>` or an ASP.NET Core exception
handler to map it to `429 Too Many Requests`.

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

## Mediarq.Aspire — .NET Aspire ServiceDefaults integration

```csharp
// MyApp.ServiceDefaults/Extensions.cs — additive, alongside the template's own setup
public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
{
    builder.ConfigureOpenTelemetry();   // generated by dotnet new aspire-servicedefaults
    builder.AddDefaultHealthChecks();   // generated by dotnet new aspire-servicedefaults

    builder.AddMediarqServiceDefaults(typeof(Program).Assembly); // wires Mediarq's tracing/metrics + handler-registration health check

    return builder;
}
```
Not a replacement for the Aspire template's own `ServiceDefaults` project — call it from inside yours.
Adds `Mediarq.OpenTelemetry`'s tracing/metrics and `Mediarq.HealthChecks`' handler-registration check on
top of whatever OpenTelemetry/service-discovery/resilience setup the template already generated; it
doesn't reimplement any of that, nor map `/health`/`/alive` endpoints itself.

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

## Mediarq.Dapr — pub/sub for containers/Kubernetes

```csharp
builder.Services.AddDaprClient();                          // from Dapr.AspNetCore/Dapr.Client
builder.Services.AddMediarqDaprPubSub<OrderPlaced>();       // publish (outbound)
builder.Services.AddMediarqDaprPubSubSubscriptions();       // subscribe (inbound) registry
// ...
app.MapDaprPubSubSubscription<OrderPlaced>();               // POST /dapr/pubsub/OrderPlaced
app.MapDaprPubSubSubscribeEndpoint();                       // GET  /dapr/subscribe (map last)
```
Marker: `IDaprPubSubEvent` (`static abstract string PubsubName`/`Topic` — static, not instance members,
so the subscribe side has routing info before any instance exists, and both directions can never drift
apart). Publishing forwards through `DaprClient.PublishEventAsync`, same "runs alongside your in-process
handlers" shape as `Mediarq.MassTransit`. Subscribing extracts the `data` field from the CloudEvents 1.0
envelope Dapr delivers and republishes it via `IPublisher` — the same pipeline as any in-process `Publish`.

## Mediarq.AzureServiceBus — a lightweight, direct broker bridge

```csharp
builder.Services.AddSingleton(_ => new ServiceBusClient(connectionString));
builder.Services.AddMediarqAzureServiceBusPublisher<OrderPlaced>();   // publish (outbound)
builder.Services.AddMediarqAzureServiceBusSubscriber<OrderPlaced>();  // subscribe (inbound background processor)
```
Marker: `IAzureServiceBusEvent` (`static abstract string TopicName`/`SubscriptionName` — same static-member
rationale as `Mediarq.Dapr`'s `IDaprPubSubEvent`). This package never owns the `ServiceBusClient`'s lifecycle,
and does **not** provision the topic/subscription — create them ahead of time (portal, ARM/Bicep, or
`ServiceBusAdministrationClient`). The subscriber completes a message only after a successful
`IPublisher.Publish`; a failure dead-letters it (Service Bus's analogue of "nack without requeue") and does
**not** dedupe redeliveries — enable the entity's built-in duplicate-detection window if you need broker-side
dedup. A lightweight alternative to `Mediarq.MassTransit` for the simple pub/sub case, same as `Mediarq.RabbitMQ`.
## Mediarq.RabbitMQ — a lightweight, direct broker bridge

```csharp
builder.Services.AddSingleton<IConnection>(_ =>
{
    var factory = new ConnectionFactory { Uri = new Uri("amqp://guest:guest@localhost:5672") };
    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});
builder.Services.AddMediarqRabbitMqPublisher<OrderPlaced>();   // publish (outbound)
builder.Services.AddMediarqRabbitMqSubscriber<OrderPlaced>();  // subscribe (inbound background consumer)
```
Marker: `IRabbitMqEvent` (`static abstract string Exchange`/`Queue`/`RoutingKey` — static for the same
reason as `Mediarq.Dapr`'s `IDaprPubSubEvent`: the subscriber declares its queue/binding at startup,
before any instance exists). This package never owns the connection's lifecycle — register an
`IConnection` yourself. The subscriber acks only after a successful `IPublisher.Publish`; a failure nacks
without requeue (route to a dead-letter exchange at the broker if you need one) and does **not** dedupe
redeliveries — for `Mediarq.MassTransit`'s heavier, batteries-included alternative (retry, outbox,
saga integration, many transports), see above.

## Mediarq.Grpc — direct point-to-point RPC to another service

```csharp
// publish (outbound)
builder.Services.AddMediarqGrpcPublisher<OrderPlaced>();

// subscribe (inbound)
builder.Services.AddMediarqGrpcSubscriptions();
// ...
app.MapMediarqGrpcNotificationService();     // map exactly once
app.MapMediarqGrpcSubscription<OrderPlaced>();
```
Marker: `IGrpcNotificationEvent` (`static abstract string ServiceAddress`). Unlike the broker packages
above, gRPC is direct point-to-point RPC — the publisher must know the target service's address, there
is no fan-out or persistence, and a failed `Publish` RPC throws `Grpc.Core.RpcException` like any other
handler exception. Ships its own compiled Protobuf/gRPC contract, so no `protoc`/`Grpc.Tools` is needed
downstream. Every subscribed notification type is multiplexed over one shared RPC method; an
unrecognized `type_name` fails with `StatusCode.NotFound`.

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
