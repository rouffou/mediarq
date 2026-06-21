# Concepts — what to use, and when

New to CQRS or to Mediarq? This page explains the handful of ideas the whole library is built on.
You only need these five.

## 1. Three kinds of message

| You want to… | Use | Returns | Handlers |
|---|---|---|---|
| **Change state** (create/update/delete) | **Command** — `ICommand<Result<T>>` or `ICommand` (no result) | a `Result` / `Result<T>`, or nothing | exactly **one** |
| **Read** something | **Query** — `IQuery<Result<T>>` | a `Result<T>` | exactly **one** |
| **Announce that something happened** | **Notification** — `INotification` | nothing | **zero or more** |

A command/query is a *request*: it has one handler and you get its answer back. A notification is an
*event*: you publish it and every interested handler reacts, independently.

```csharp
public record CreateOrder(string Customer) : ICommand<Result<Guid>>;   // one handler, returns the new id
public record GetOrder(Guid Id)            : IQuery<Result<OrderDto>>; // one handler, returns the order
public record OrderPlaced(Guid Id)         : INotification;            // many handlers, returns nothing
```

> Rule of thumb: if two different things must happen after an event (send an email **and** update a
> read model), that's a **notification** with two handlers — not one command that does both.

## 2. Who dispatches them

Inject one of these (they all come from the DI container after registration):

- `ISender` — sends commands and queries: `Send(...)`, and `CreateStream(...)` for streaming.
- `IPublisher` — publishes notifications: `Publish(...)`.
- `IMediator` — both at once (it *is* `ISender` + `IPublisher`).

Inject the **narrowest** one a class needs. A controller that only sends commands takes `ISender`.

## 3. Handlers

A handler is where your logic lives. One interface per message kind:

```csharp
public class CreateOrderHandler : ICommandHandler<CreateOrder, Result<Guid>>
{
    public Task<Result<Guid>> Handle(CreateOrder request, CancellationToken ct = default) { /* ... */ }
}
```

Handlers are registered for you (see [Wiring extensions](wiring-extensions.md) for the two ways) and
resolved from DI, so you can inject repositories, a `DbContext`, `ILogger`, etc. through the constructor.

## 4. `Result` instead of exceptions

Mediarq is *railway-oriented*: expected failures (not found, validation, conflict) are returned as a
**failed `Result`**, not thrown. Exceptions stay for the truly exceptional.

```csharp
Result<Order> r = await sender.Send(new GetOrder(id));
if (r.IsSuccess) Use(r.Value);
else             Console.WriteLine(r.Error.Message);   // r.Error.Type ∈ {NotFound, Validation, Conflict, …}
```

- `Result.Success()` / `Result.Success(value)` — success (optionally carrying a value).
- `Result.Failure(error)` / `Result.Failure<T>(error)` — failure carrying a `ResultError`.
- `ResultError.NotFound(code, msg)`, `.Conflict(...)`, `.Failure(...)`, `.Problem(...)` — typed errors.

Compose results without `if` ladders: `Map`, `Bind`, `Ensure`, `Match`, `Tap`. With
[Mediarq.AspNetCore](wiring-extensions.md), a `Result` maps straight to an HTTP response (`200`/`Ok`
or RFC 7807 `ProblemDetails`).

## 5. The pipeline

Every `Send` goes through a **pipeline** of *behaviors* that wrap the handler — validation, logging,
your own cross-cutting logic — then the handler runs:

```
Send(request)
  → [exception handling] → [validation] → [your behaviors] → handler → back out
```

You rarely touch this: validation, exception handling and pre/post processors light up automatically
when you add a validator / handler / processor. Add your own with `IPipelineBehavior<,>`
(see [Writing a behavior](writing-a-behavior.md)).

## Where next

- [Your first app](your-first-app.md) — build a working API step by step.
- [Wiring extensions](wiring-extensions.md) — register the core and each optional package.
- [Troubleshooting](troubleshooting.md) — when something silently doesn't fire.
