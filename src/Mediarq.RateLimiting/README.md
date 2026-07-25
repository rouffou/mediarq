# Mediarq.RateLimiting

Rate limiting as a pipeline behavior, built on `System.Threading.RateLimiting`: mark a command/query as
`IRateLimitedRequest`, and it acquires a permit from a named policy before its handler runs — no global
middleware, no HTTP dependency.

```bash
dotnet add package Mediarq.RateLimiting
```

## Usage

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

```csharp
public record CreateOrder(string Customer) : ICommand<Result<Guid>>, IRateLimitedRequest
{
    public string PolicyName => "orders-per-user";
    public string? PartitionKey => Customer; // independent limit per customer; omit for a single shared bucket
}
```

`key` in `AddPolicy` receives `PartitionKey` (or `"*"` when `null`) — build any `RateLimitPartition<string>`
from it: `GetFixedWindowLimiter`, `GetSlidingWindowLimiter`, `GetTokenBucketLimiter`, `GetConcurrencyLimiter`,
`GetNoLimiter`, all from `System.Threading.RateLimiting`.

## Handling a rejection

No permit available → `RateLimitExceededException` (`PolicyName`, `PartitionKey`, `RetryAfter` when the
limiter reports one). Catch it with an `IRequestExceptionHandler<TRequest, TResponse>` to turn it into a
typed `Result` failure, or with an ASP.NET Core exception handler to return `429 Too Many Requests`.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
