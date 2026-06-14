# Changelog

All notable changes to Mediarq are documented in this file. The format is based on
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project adheres to
[Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Multi-targeting for **.NET 8, 9 and 10**.
- Source generator (`Mediarq.SourceGenerators`) for compile-time, reflection-free registration via
  `AddMediarqCore()` + the generated `AddMediarqHandlers()` (trimming/AOT friendly). Emits diagnostic
  `MQ001` when multiple handlers are registered for the same request.
- `ISender` / `IPublisher` split of `IMediator` (interface segregation).
- Configurable notification publishing via `INotificationPublisher`
  (`ParallelNotificationPublisher` default, `SequentialNotificationPublisher`, or custom).
- Functional `Result` combinators: `Map`, `Bind`, `Match`, `Tap`, `Ensure`, plus async variants
  (`MapAsync`, `BindAsync`, `MatchAsync`, `TapAsync`).
- `Unit` type; no-result commands (`ICommand`) now flow through the full pipeline.
- Asynchronous validators (`IValidator<T>.ValidateAsync`).
- `Mediarq.FluentValidation` package: adapter to run FluentValidation validators in the pipeline.
- `Mediarq.AspNetCore` package: map `Result` / `ResultError` to `IResult` and RFC 7807 ProblemDetails.
- NuGet metadata, XML documentation, SourceLink and symbol packages (`snupkg`).
- GitHub Actions CI (build + test) and release (pack + publish on `v*` tags).
- Comprehensive README and a BenchmarkDotNet project comparing Mediarq with MediatR.

### Changed
- Mediator dispatch no longer uses `dynamic` or per-call reflection (cached, strongly-typed wrappers).
- `IRequestContextFactory.Create` returns a typed `RequestContext<TRequest, TResponse>`.
- Nullable reference types enabled across the library.
- `PerformanceBehavior` measures time through the injected `IClock`.
- `PipelineExecutor` honors `IOrderBehavior` ordering.
- Configurable assembly scanner (`AddMediarq(isHttp, params Assembly[])`); notification handlers are
  now registered by the scan.

### Fixed
- `LoggingBehavior` now awaits the handler before logging completion and sets `FinishedAt`.
- `INotificationHandler<T>` is type-safe (`Handle(TNotification)`).
- Removed the silent single-handler fallback in `HandlerResolver.ResolveAll`.
- Renamed `IIMMutableRequestContext` to `IMutableRequestContext`.
- Removed an erroneous `System.Windows.Input` import.

## [1.0.0]
- Initial public release.
