# Mediarq.MediatRCompat

An optional compatibility shim so you can migrate to Mediarq **incrementally**: keep your existing
`MediatR.IRequest`/`IRequestHandler`/`INotification`/`INotificationHandler`/`IStreamRequest`/
`IStreamRequestHandler` classes completely unchanged, wire them through Mediarq's own dispatch pipeline,
and convert handlers to native Mediarq types file by file (e.g. with `Mediarq.Analyzers`) at your own pace.

```bash
dotnet add package Mediarq.MediatRCompat
```

## Usage

Register it **in addition to** (after) `AddMediarq`/`AddMediarqCore`, which must already have configured
Mediarq's own dispatch services:

```csharp
builder.Services.AddMediarq(isHttp: true, typeof(Program).Assembly)
                 .AddMediarqMediatRCompat(typeof(Program).Assembly);
```

`AddMediarqMediatRCompat` scans the given assemblies for classes implementing MediatR's own
`IRequestHandler<,>` / `IRequestHandler<>` / `INotificationHandler<>` / `IStreamRequestHandler<,>`, and
registers each behind a `MediatRCompatMediator` that dispatches through Mediarq's real
`ISender`/`IPublisher` — so your existing handlers run through the same pipeline behaviors (validation,
logging, ...) as native Mediarq requests.

Inject `MediatR.IMediator` (or `ISender`/`IPublisher`) exactly as you did with MediatR — no source change
needed in the classes that send requests or publish notifications, either:

```csharp
public sealed class Ping(string Message) : MediatR.IRequest<string>;

public sealed class PingHandler : MediatR.IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken ct) => Task.FromResult(request.Message);
}

// Unchanged call site:
var response = await mediator.Send(new Ping("hi"));
```

The runtime-typed `Send(object)` / `Publish(object)` / `CreateStream(object)` overloads MediatR itself
exposes are supported too (useful for generic infrastructure code that doesn't know the request type at
compile time).

## What isn't bridged

This shim covers requests, handlers, notifications and streaming — the same surface
`Mediarq.Analyzers`' migration analyzer targets. It does **not** bridge MediatR's `IPipelineBehavior<,>`,
`IRequestPreProcessor<>`/`IRequestPostProcessor<,>`, or `IRequestExceptionHandler<,,>`: their signatures
differ enough from Mediarq's own that converting them is left as a manual step even in a full rewrite —
see [Migrating from MediatR](https://github.com/rouffou/mediarq/blob/main/docs/guides/migrating-from-mediatr.md).
Write any new cross-cutting behavior directly against Mediarq's `IPipelineBehavior<,>`.

## Note on performance

Because a MediatR-shaped request's concrete type is only known at runtime, dispatching it requires
reflection once per concrete type (the result is cached — subsequent calls for the same type are
reflection-free). This is an explicit migration/compatibility path, not the Native-AOT hot path: convert
a handler to a native Mediarq type when you want fully reflection-free dispatch for it.

## Learn more

[Mediarq.Analyzers](https://www.nuget.org/packages/Mediarq.Analyzers) ·
[Migrating from MediatR](https://github.com/rouffou/mediarq/blob/main/docs/guides/migrating-from-mediatr.md) ·
[Full README](https://github.com/rouffou/mediarq#readme)

MIT © Nicolas Rouffart
