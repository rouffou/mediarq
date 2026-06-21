# Native AOT & trimming

Mediarq is built to run under **Native AOT** and aggressive trimming. The core library is marked
`IsAotCompatible` and produces **zero IL trim/AOT warnings**. The key is to avoid the runtime
assembly scan and use the source-generated registration instead.

## Use the generated registration

The `AddMediarq(...)` overload scans assemblies with reflection and is annotated
`[RequiresUnreferencedCode]`. For AOT, register the core services and call the generated
`AddMediarqHandlers()`:

```csharp
services.AddMediarqCore()
        .AddMediarqHandlers(); // emitted at compile time by the Mediarq source generator
```

`AddMediarqHandlers()` is produced by the analyzer shipped inside the package. It registers every
handler, behavior and validator it finds at compile time, and pre-populates a `MediarqWrapperRegistry`
with strongly-typed dispatch wrappers — so `Send`, `Publish` and `CreateStream` never need
`Activator.CreateInstance` or `MakeGenericType` at runtime.

## Publishing for Native AOT

```bash
dotnet publish -c Release -r win-x64 /p:PublishAot=true
```

There is no extra Mediarq configuration to add: as long as you use `AddMediarqCore()` +
`AddMediarqHandlers()`, the dispatch path is reflection-free.

## Generator options (MSBuild)

The generated `AddMediarqHandlers()` is `internal` and lives in the `Mediarq.Extensions` namespace by
default. Override either:

```xml
<PropertyGroup>
  <!-- Make it callable from another assembly. -->
  <MediarqGeneratedAccessibility>public</MediarqGeneratedAccessibility>
  <!-- Choose the namespace of the generated registration class. -->
  <MediarqGeneratedNamespace>MyApp.Generated</MediarqGeneratedNamespace>
</PropertyGroup>
```

## Things to watch on AOT

- **JSON in distributed caching.** `Mediarq.Caching`'s default `IMediarqCacheSerializer` uses
  reflection-based `System.Text.Json`. On AOT, register your own `IMediarqCacheSerializer` backed by a
  `JsonSerializerContext` (source-generated).
- **FluentValidation / DataAnnotations.** These integrations use the respective libraries; check their
  own AOT guidance. The Mediarq `IValidator<T>` abstraction itself is reflection-free.
- **Custom reflection in handlers.** Mediarq does not add reflection to your handlers — but your own
  code (or third-party libraries) still needs to be AOT-friendly.

## Verifying

The repository ships an AOT smoke sample under `Samples/Mediarq.AotSample`, and a CI job publishes it
with `PublishAot=true` to catch regressions. Run a trim/AOT analysis on your app with:

```bash
dotnet publish -c Release -r <rid> /p:PublishAot=true
```

and treat any `ILxxxx` warnings originating from your usage as actionable.
