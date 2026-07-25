# Mediarq.Deferred

In-process deferred dispatch: `SendLaterAsync`/`PublishLaterAsync` queue a command or notification on a
`System.Threading.Channels`-backed background worker instead of running its handler(s) inline —
decoupling the caller from handler execution time. No external dependency, no persistent store: fills the
gap between immediate `Send`/`Publish` and durable scheduling (`Mediarq.Hangfire`/`Mediarq.Quartz`) for
the simple "reliable in-process fire-and-forget" case.

```bash
dotnet add package Mediarq.Deferred
```

## Usage

```csharp
builder.Services.AddMediarqDeferredDispatch();
```

```csharp
public sealed class OrdersController(IDeferredDispatcher deferred)
{
    public async Task<IActionResult> Post(CreateOrder command)
    {
        await deferred.SendLaterAsync(command);       // returns immediately; a background worker dispatches it
        return Accepted();
    }
}
```

`PublishLaterAsync` works the same way for notifications:

```csharp
await deferred.PublishLaterAsync(new OrderPlaced(orderId));
```

Both queue a closure capturing the concrete request; a single background worker (`DeferredDispatchHostedService`)
drains the queue, running each item through the real `ISender`/`IPublisher` pipeline (validation, other
behaviors, handlers) in its own DI scope. An exception in one item is logged and does not stop the worker
from processing the next one.

## What "reliable" means here

Queued work is **in-memory only** — it does not survive a process crash or restart. What it does guarantee
is a graceful drain: on a normal host shutdown, the worker stops accepting new work and finishes everything
already queued (bounded by the host's own shutdown timeout) instead of dropping it. If you need the item to
survive a crash, or to run at a specific future time / on a cron schedule, use `Mediarq.Hangfire` or
`Mediarq.Quartz` instead.

## Backpressure

By default the queue is unbounded. Cap it with `AddMediarqDeferredDispatch(o => { o.Capacity = 1000; o.FullMode = BoundedChannelFullMode.Wait; })`
to apply backpressure to callers once full — see `System.Threading.Channels.BoundedChannelFullMode` for the
other modes (`DropOldest`, `DropNewest`, `DropWrite`).

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
