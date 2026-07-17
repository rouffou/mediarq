# MediatR parity audit

This page enumerates MediatR's capabilities and how Mediarq covers each one — supported as-is, supported
differently, or intentionally omitted. It is the companion to [Migrating from MediatR](migrating-from-mediatr.md),
which shows the mechanical type mapping (and the analyzer that automates it).

Legend: ✅ supported · ⚠️ supported with a deliberate difference · ❌ not provided (by design)

## Requests & handlers

| MediatR | Mediarq | Status |
|---|---|---|
| `IRequest<TResponse>` | `ICommand<TResponse>` / `IQuery<TResponse>` (both `ICommandOrQuery<TResponse>`) | ⚠️ split into command vs query |
| `IRequest` (void) | `ICommand` (flows as `Unit`) | ✅ |
| `IRequestHandler<TRequest, TResponse>` | `IRequestHandler<,>` (or `ICommandHandler<,>` / `IQueryHandler<,>`) | ✅ |
| `IRequestHandler<TRequest>` (returns `Unit`) | `IRequestHandler<TRequest>` / `ICommandHandler<TRequest>` | ✅ |
| `ISender.Send<TResponse>(IRequest<TResponse>)` | `ISender.Send<TResponse>(ICommandOrQuery<TResponse>)` | ✅ |
| `ISender.Send(object request)` | — | ❌ runtime-typed dispatch needs reflection; omitted for the AOT-friendly design. Call the generic overload. |

## Notifications

| MediatR | Mediarq | Status |
|---|---|---|
| `INotification` / `INotificationHandler<T>` | same names | ✅ |
| `IPublisher.Publish<T>(T)` | `IPublisher.Publish<T>(T) where T : INotification` | ✅ |
| `IPublisher.Publish(object notification)` | — | ❌ omitted (reflection); use the generic overload |
| `INotificationPublisher` (`ForeachAwait`, `TaskWhenAll`) | `INotificationPublisher` (`Sequential`, `Parallel` default, `AggregateException`) | ✅ richer set |
| Ordered handler execution | `IOrderedNotificationHandler` (ascending `Order`) | ✅ Mediarq addition |

## Pipeline

| MediatR | Mediarq | Status |
|---|---|---|
| `IPipelineBehavior<TRequest, TResponse>` | `IPipelineBehavior<TRequest, TResponse>` | ⚠️ different `Handle` signature (see below) |
| `RequestHandlerDelegate<TResponse> next` | `Func<Task<TResponse>> handle` + `IMutableRequestContext` | ⚠️ |
| `IRequestPreProcessor<TRequest>` | `IRequestPreProcessor<TRequest>` | ✅ |
| `IRequestPostProcessor<TRequest, TResponse>` | `IRequestPostProcessor<TRequest, TResponse>` | ✅ |
| `IRequestExceptionHandler<TRequest, TResponse, TException>` | `IRequestExceptionHandler<TRequest, TResponse>` | ✅ (filter on the exception type inside `Handle`) |
| `IRequestExceptionAction<TRequest, TException>` (observe only) | — | ❌ no observe-only variant; use an exception handler that re-marks unhandled, or a behavior |
| Behavior ordering | `IOrderBehavior` (opt-in) + registration order | ✅ |

### The `IPipelineBehavior` signature difference

MediatR passes the continuation as `RequestHandlerDelegate<TResponse> next` and the request as the first
argument. Mediarq passes an `IMutableRequestContext<TRequest, TResponse>` (request, user, correlation id,
items bag) and the continuation as `Func<Task<TResponse>> handle`:

```csharp
// MediatR
public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
{
    var response = await next();
    return response;
}

// Mediarq
public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken ct = default)
{
    var request = context.Request;     // when you need the request
    var response = await handle();
    return response;
}
```

The migration analyzer rewrites the interface reference but **leaves the method body to you** — adjust
`next` → `handle` and read the request from `context.Request`.

## Streaming

| MediatR | Mediarq | Status |
|---|---|---|
| `IStreamRequest<T>` / `IStreamRequestHandler<,>` | same names | ✅ |
| `ISender.CreateStream<T>(IStreamRequest<T>)` | `ISender.CreateStream<T>(IStreamRequest<T>)` | ✅ |
| `ISender.CreateStream(object request)` | — | ❌ omitted (reflection) |
| `IStreamPipelineBehavior<TRequest, TResponse>` | `IStreamPipelineBehavior<TRequest, TResponse>` | ✅ |

## Registration

| MediatR | Mediarq | Status |
|---|---|---|
| `AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...))` | `AddMediarq(isHttp, params Assembly[])` (scan) | ✅ |
| Reflection-free / source-generated registration | `AddMediarqCore()` + generated `AddMediarqHandlers()` | ✅ Mediarq addition (AOT-safe) |
| Handler lifetime configuration | `[RegisterHandler(ServiceLifetime)]` per handler | ⚠️ per-handler instead of global |
| Custom `INotificationPublisher` | register your own `INotificationPublisher` | ✅ |
| Open-generic handler registration | supported via scan / generator | ✅ |

## What Mediarq adds beyond MediatR

- **`Result` / `Result<T>`** railway-oriented type with combinators (`Map`, `Bind`, `Match`, `Ensure`, …)
  and ASP.NET Core mapping (`ToHttpResult`, `ToActionResult`).
- **Source generator** for reflection-free, trimming/Native-AOT-safe dispatch (0 trim/AOT warnings).
- **Lean-by-default pipeline**: built-in behaviors register only when applicable; logging/performance are opt-in.
- **First-class validation** (`IValidator<T>`, FluentValidation and DataAnnotations adapters), including
  notification validation.
- **`ISender` / `IPublisher` split** of `IMediator` for interface segregation.
- **Reliability building blocks**: caching, idempotency, transactional outbox, unit of work, resilience (Polly),
  diagnostics/OpenTelemetry, MassTransit forwarding — all as opt-in packages.

## Summary of the deliberate gaps

Everything MediatR offers has a Mediarq equivalent **except** the runtime-typed `object` overloads
(`Send(object)`, `Publish(object)`, `CreateStream(object)`) and the observe-only
`IRequestExceptionAction`. The `object` overloads are omitted on purpose: they require reflection, which
is at odds with Mediarq's AOT-friendly, source-generated dispatch. If you genuinely need dynamic dispatch,
resolve the handler through `IServiceProvider` yourself or keep a small reflection shim in your app.
