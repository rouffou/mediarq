# Mediarq

A lightweight, dependency-free **CQRS mediator for .NET** — a free alternative to MediatR with
commands, queries, notifications, a composable pipeline of behaviors, built-in validation and
`Result` types. Designed for domain-driven and CQRS architectures.

- ✅ Commands / queries returning a `Result` (railway-oriented), no-result commands, notifications
- ✅ Streaming requests (`IStreamRequest<T>` → `IAsyncEnumerable<T>`)
- ✅ Pipeline behaviors (logging, performance, validation, exception handling) + your own, orderable
- ✅ Reflection-free dispatch via a source generator — **Native AOT** friendly
- ✅ Functional `Result` combinators (`Map`, `Bind`, `Match`, `Tap`, `Ensure`)

## Packages

| Package | Purpose |
|---|---|
| `Mediarq` | Core mediator, pipeline, results, source generator |
| `Mediarq.AspNetCore` | Map `Result` → `IResult` / ProblemDetails |
| `Mediarq.FluentValidation` | Run FluentValidation validators in the pipeline |
| `Mediarq.DataAnnotations` | Validate requests with `System.ComponentModel.DataAnnotations` |
| `Mediarq.Caching` | Memoize `ICacheableRequest` responses (in-memory or distributed) |
| `Mediarq.Diagnostics` | Tracing (`Activity`) + metrics, OpenTelemetry-compatible |
| `Mediarq.OpenTelemetry` | One-line `AddMediarqInstrumentation()` for the OTel providers |
| `Mediarq.UnitOfWork` | Commit a unit of work around transactional commands |
| `Mediarq.EntityFrameworkCore` | `IUnitOfWork` over a `DbContext` |
| `Mediarq.Polly` | Resilience (retry/timeout/circuit breaker) via Polly |
| `Mediarq.MassTransit` | Publish notifications out-of-process on a MassTransit bus |

## Guides

- [Migrating from MediatR](guides/migrating-from-mediatr.md)
- [Native AOT & trimming](guides/native-aot.md)
- [Writing a behavior](guides/writing-a-behavior.md)

## Getting started

See the [README](https://github.com/rouffou/mediarq#readme) for usage, and the **API** section for
the full reference generated from the XML documentation.
