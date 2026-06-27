# Mediarq.Polly

Execute a request through a named [Polly](https://www.pollydocs.org) resilience pipeline (retry, timeout,
circuit breaker, …) by implementing `IResilientRequest`.

```bash
dotnet add package Mediarq.Polly
```

## Usage

```csharp
builder.Services.AddResiliencePipeline("orders-pricing", p => p
    .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 3 }));
builder.Services.AddMediarqResilience(); // after AddMediarq / AddMediarqCore
```

```csharp
public record QuotePrice(Guid OrderId) : IQuery<Result<decimal>>, IResilientRequest
{
    public string ResiliencePipelineName => "orders-pricing";
}
```

The handler is run through the matching pipeline, so transient failures are retried transparently.

> ⚠️ The `ResiliencePipelineName` returned by the request **must match** a pipeline registered with
> `AddResiliencePipeline("that-name", …)`, or resolution throws at dispatch.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
