# Mediarq.OpenTelemetry

One-line wiring to export Mediarq's traces and metrics through OpenTelemetry. Subscribes the tracer and
meter providers to the `"Mediarq"` source/meter emitted by **Mediarq.Diagnostics**.

```bash
dotnet add package Mediarq.OpenTelemetry
```

## Usage

```csharp
builder.Services.AddMediarqDiagnostics(); // emits the Activity/metrics (after AddMediarq/AddMediarqCore)

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddMediarqInstrumentation().AddOtlpExporter())
    .WithMetrics(metrics => metrics.AddMediarqInstrumentation().AddOtlpExporter());
```

`AddMediarqInstrumentation()` is just `AddSource("Mediarq")` / `AddMeter("Mediarq")` — pair it with any
exporter (OTLP, Console, …). Without **Mediarq.Diagnostics** there is nothing to export.

## Learn more

[Wiring extensions](https://github.com/rouffou/mediarq/blob/main/docs/guides/wiring-extensions.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
