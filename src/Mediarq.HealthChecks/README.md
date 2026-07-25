# Mediarq.HealthChecks

Catches a missing or ambiguous handler **before** it becomes a `HandlerNotFoundException` at the first
dispatch in production: an `IHealthCheck` for your `/health` endpoint, plus an optional check that runs
once at host startup and fails the app fast.

```bash
dotnet add package Mediarq.HealthChecks
```

## Usage

```csharp
builder.Services.AddHealthChecks()
    .AddMediarqHandlerRegistrationCheck(assemblies: typeof(Program).Assembly);
```

Exposes it the usual way:

```csharp
app.MapHealthChecks("/health");
```

`Healthy` when every `ICommand`/`IQuery` closed type discovered in `assemblies` resolves to exactly one
registered `IRequestHandler<TRequest, TResponse>`; otherwise `Unhealthy`, with the offending types and
their handler counts (0 = missing, 2+ = ambiguous) in the result's `Data`.

## Fail fast at startup

```csharp
builder.Services.AddMediarqHandlerValidationOnStartup(typeof(Program).Assembly);
```

Runs the same check once, synchronously, as part of host startup — a misconfiguration throws
`InvalidOperationException` and the app never starts accepting traffic, instead of surfacing on whichever
request happens to hit the broken handler first.

`assemblies` defaults to the entry assembly when omitted; pass every assembly that declares
commands/queries or handlers explicitly in a modular app.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
