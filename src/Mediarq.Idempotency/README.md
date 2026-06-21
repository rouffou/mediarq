# Mediarq.Idempotency

Run a request **at most once per key**. A repeated request that implements `IIdempotentRequest` with the
same key replays the stored result instead of re-executing the handler — so a retried POST won't double
its effect.

```bash
dotnet add package Mediarq.Idempotency
```

## Usage

```csharp
builder.Services.AddDistributedMemoryCache();   // REQUIRED: an IDistributedCache (use Redis in production)
builder.Services.AddMediarqIdempotency();
```

```csharp
public record ConfirmOrder(Guid OrderId, string IdempotencyKey) : ICommand<Result>, IIdempotentRequest
{
    public TimeSpan? IdempotencyDuration => TimeSpan.FromMinutes(10);
}
```

```csharp
// e.g. read the key from an Idempotency-Key header
var key = http.Headers["Idempotency-Key"].FirstOrDefault() ?? Guid.NewGuid().ToString();
await sender.Send(new ConfirmOrder(id, key));
```

Call after `AddMediarq` / `AddMediarqCore`. Use a stable, caller-supplied key. The stored result is
JSON-serialized; `Result` / `Result<T>` round-trip out of the box. For strict once-only semantics under
high concurrency, back it with a store that supports atomic set-if-absent (e.g. Redis).

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
