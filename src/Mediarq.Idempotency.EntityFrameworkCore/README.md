# Mediarq.Idempotency.EntityFrameworkCore

An `IDistributedCache` backed by EF Core, so **Mediarq.Idempotency** can persist replay results in your
own database instead of requiring Redis.

```bash
dotnet add package Mediarq.Idempotency.EntityFrameworkCore
```

## Usage

1. Map the cache table in your `DbContext`:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
    => modelBuilder.ApplyMediarqIdempotencyCache();
```

2. Register it **instead of** `AddDistributedMemoryCache()` / `AddStackExchangeRedisCache()`, before
   `AddMediarqIdempotency()`:
```csharp
builder.Services.AddMediarqIdempotencyEntityFrameworkCore<AppDbContext>(o => o.CleanupInterval = TimeSpan.FromMinutes(10));
builder.Services.AddMediarqIdempotency();
```

That's it — `IdempotencyBehavior` already depends on the standard `IDistributedCache` abstraction, so it
works unchanged against this implementation. A background `IdempotencyCacheCleanupService` periodically
removes expired entries (already-expired entries are also skipped on read, so this is just storage
hygiene for keys that are never read again after expiring).

## Note on lifetime

`EfCoreDistributedCache<TContext>` is registered **scoped**, tied to the same `TContext` instance as the
rest of the request — unlike the usual singleton `IDistributedCache` implementations. This is safe for
`IdempotencyBehavior` (itself scoped), but don't share this registration with a singleton or with a
general-purpose cache used elsewhere in your app.

## Learn more

[Mediarq.Idempotency](https://www.nuget.org/packages/Mediarq.Idempotency) ·
[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
