# Mediarq.Diagnostics

Emit an `Activity` (trace) and metrics (request count + duration) for every dispatched request, under the
`"Mediarq"` source/meter — OpenTelemetry-compatible.

```bash
dotnet add package Mediarq.Diagnostics
```

## Usage

```csharp
builder.Services.AddMediarqDiagnostics(); // call AFTER AddMediarq / AddMediarqCore
```

> ⚠️ Register it **after** the core: it decorates the notification publisher, which must already be
> registered.

Collect the data with OpenTelemetry (see **Mediarq.OpenTelemetry** for one-line wiring) or any
`System.Diagnostics` listener:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(t => t.AddMediarqInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddMediarqInstrumentation().AddOtlpExporter());
```

Instruments: `mediarq.requests.count` and `mediarq.requests.duration`.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
