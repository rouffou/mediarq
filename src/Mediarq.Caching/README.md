# Mediarq.Caching

Memoize the response of a request that implements `ICacheableRequest` — in-memory (`IMemoryCache`) or
distributed (`IDistributedCache` / Redis). On a cache hit the handler doesn't run.

```bash
dotnet add package Mediarq.Caching
```

## Usage

```csharp
builder.Services.AddMediarqCaching();            // in-memory (registers IMemoryCache for you)
// or distributed:
builder.Services.AddDistributedMemoryCache();    // or AddStackExchangeRedisCache(...)
builder.Services.AddMediarqDistributedCaching();
```

```csharp
public record GetOrder(Guid Id) : IQuery<Result<OrderDto>>, ICacheableRequest
{
    public string CacheKey => $"orders:{Id}";
    public TimeSpan? CacheDuration => TimeSpan.FromSeconds(30);
}
```

Call after `AddMediarq` / `AddMediarqCore`. Distributed caching serializes the response to JSON —
`Result` / `Result<T>` round-trip out of the box; keep your DTOs serializable. On Native AOT, register a
source-generated `IMediarqCacheSerializer`.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
