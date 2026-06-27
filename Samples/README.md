# Mediarq — Samples

Three runnable projects that together cover the whole library surface, from a 5-minute tour of the
core to a web app that wires up every extension.

| Sample | Kind | What it shows | Run |
|---|---|---|---|
| [Mediarq.Samples.Quickstart](Mediarq.Samples.Quickstart) | Console | The core, in-process: commands / queries / void commands, notifications (multiple handlers), streaming, built-in validation, a custom ordered behavior, pre/post processors, exception → `Result`, timeout, and `Result` combinators. | `dotnet run --project Samples/Mediarq.Samples.Quickstart` |
| [Mediarq.Samples.WebApi](Mediarq.Samples.WebApi) | ASP.NET Core (minimal API) | A small **Orders** domain wiring the extensions end-to-end: `Result` → HTTP/ProblemDetails, FluentValidation **and** DataAnnotations, caching, idempotency, EF Core unit of work, transactional outbox, Polly resilience, diagnostics + OpenTelemetry, and MassTransit forwarding. | `dotnet run --project Samples/Mediarq.Samples.WebApi` |
| [Mediarq.AotSample](Mediarq.AotSample) | Console (Native AOT) | The **reflection-free** path (`AddMediarqCore().AddMediarqHandlers()`): command, query, streaming, source-generated validation failures, and a `[RegisterHandler]` lifetime override — publishes cleanly with Native AOT. | `dotnet run --project Samples/Mediarq.AotSample` |

## Quickstart (console)

A guided tour printed section by section — the easiest place to start. It uses the convenient
assembly-scan registration `AddMediarq(...)` and the opt-in logging / performance / timeout behaviors,
so you can see the pipeline wrap each handler in the console output.

## WebApi (Orders)

An ASP.NET Core minimal API around an in-memory EF Core "Orders" domain. Run it and open the Scalar UI
at `/scalar/v1` to exercise the endpoints:

| Endpoint | Capability |
|---|---|
| `POST /orders` | Create — FluentValidation, unit of work commit, **transactional outbox** event |
| `GET /orders/{id}` | Read — memoized by the **caching** behavior (second identical call skips the DB) |
| `POST /orders/{id}/confirm` | **Idempotent** — send the same `Idempotency-Key` header to replay instead of re-running |
| `POST /orders/{id}/note` | **DataAnnotations** validation (an empty note returns a 400 ProblemDetails) |
| `GET /orders/{id}/quote` | **Polly** resilience — retries a flaky pricing service |
| `GET /orders/stream` | **Streaming** — `IAsyncEnumerable` of orders |

Watch the console: the outbox publishes `OrderPlacedEvent` to its in-process handlers **and** forwards it
onto the in-memory MassTransit bus, and OpenTelemetry exports Mediarq activities/metrics.

> The WebApi uses the scan-based `AddMediarq(...)` for readability. For trimming / Native AOT, swap it
> for `AddMediarqCore(isHttp: true).AddMediarqHandlers()` — the path the AOT sample demonstrates.

## AOT sample

```bash
dotnet run     --project Samples/Mediarq.AotSample                 # run it
dotnet publish --project Samples/Mediarq.AotSample -r win-x64 -c Release   # native, 0 trim/AOT warnings
```

Native AOT publish needs the platform C/C++ build tools (on Windows, the "Desktop development with C++"
workload) available on PATH; the managed build and the trim/AOT analyzers run without them.
