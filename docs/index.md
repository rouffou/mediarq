# Mediarq

A lightweight, minimal-dependency **CQRS mediator for .NET** — a free alternative to MediatR with
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
| `Mediarq` | Meta-package: the core (`Mediarq.Core`) plus the common optional extensions |
| `Mediarq.Core` | Core mediator, pipeline, results, source generator |
| `Mediarq.Analyzers` | Roslyn analyzer + code fix that rewrite MediatR types to their Mediarq equivalents |
| `Mediarq.Authorization` | `IAuthorizedRequest` + policy-based authorization behavior, short-circuiting into a typed `Result` failure |
| `Mediarq.RateLimiting` | Rate-limiting behavior on `System.Threading.RateLimiting`, partitionable by request type or key |
| `Mediarq.HealthChecks` | Startup/ongoing health check: every command/query has exactly one registered handler |
| `Mediarq.Testing` | `SpyMediator`, assertion helpers, `FakeClock` / `FakeUserContext` |
| `Mediarq.Deferred` | In-process deferred dispatch (`SendLaterAsync` / `PublishLaterAsync`) on a background channel |
| `Mediarq.Hangfire` | Delayed and scheduled command dispatch via Hangfire |
| `Mediarq.Quartz` | Delayed and scheduled command dispatch via Quartz.NET |
| `Mediarq.Saga` | Saga / process-manager primitives with a persisted, correlated state |
| `Mediarq.RabbitMQ` | Direct RabbitMQ bridge for notifications |
| `Mediarq.AzureServiceBus` | Direct Azure Service Bus bridge for notifications |
| `Mediarq.Dapr` | Dapr pub/sub bridge for notifications |
| `Mediarq.Grpc` | gRPC transport for cross-service notifications |
| `Mediarq.Aspire` | .NET Aspire ServiceDefaults integration |
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
| `Mediarq.Idempotency` | Run a request at most once per key (`IIdempotentRequest`) |
| `Mediarq.Idempotency.EntityFrameworkCore` | EF Core-backed `IDistributedCache` for `Mediarq.Idempotency`, no Redis required |
| `Mediarq.Outbox` | Transactional outbox over EF Core (capture + reliable background publish) |
| `Mediarq.MediatRCompat` | Optional MediatR compatibility shim for incremental migration |

## Guides

New here? Start with **Concepts**, then build **Your first app**.

- [Concepts](guides/concepts.md) — commands vs queries vs notifications, `Result`, the pipeline
- [Your first app](guides/your-first-app.md) — a working API, step by step
- [Wiring extensions](guides/wiring-extensions.md) — register the core and each optional package
- [Writing a behavior](guides/writing-a-behavior.md) — custom cross-cutting logic
- [Testing](guides/testing.md) — unit- and integration-test handlers, validators, behaviors
- [Migrating from MediatR](guides/migrating-from-mediatr.md)
- [Native AOT & trimming](guides/native-aot.md)
- [A ready-to-use observability dashboard](guides/observability-dashboard.md) — import a Grafana dashboard for Mediarq.Diagnostics + OpenTelemetry messaging semantic conventions
- [Versioning & public API policy](guides/versioning.md) — SemVer rules and the `PublicAPI.*.txt` workflow
- [Troubleshooting](guides/troubleshooting.md) — when something silently doesn't fire

## Getting started

See the [README](https://github.com/rouffou/mediarq#readme) for usage, the **Guides** above for
task-focused walkthroughs, and the **API** section for the full reference generated from the XML
documentation.
