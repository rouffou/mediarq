# Writing a pipeline behavior

A pipeline behavior wraps the handler, so you can add cross-cutting logic (logging, auditing,
authorization, caching, …) around every matching request.

## The interface

```csharp
public sealed class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IAuditLog _audit;

    public AuditBehavior(IAuditLog audit) => _audit = audit;

    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        _audit.Starting(typeof(TRequest).Name, context.RequestId);

        var response = await handle();           // call the rest of the pipeline / the handler

        _audit.Finished(typeof(TRequest).Name);
        return response;
    }
}
```

- `context` carries metadata: `RequestId`, `CorrelationId`, `UserId`, timing, and a mutable `Items`
  bag. The identifiers are created lazily, so don't read them unless you need them.
- `handle()` runs the next behavior (or the handler). Call it exactly once for a normal flow; skip it
  to short-circuit (for example, return a cached or failed `Result`).

## Registering it

With the assembly scan, any `IPipelineBehavior<,>` in a scanned assembly is registered automatically.
With the generated registration, `AddMediarqHandlers()` picks it up at compile time. You can also
register an open generic explicitly:

```csharp
services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
```

## Ordering — `IOrderBehavior`

Behaviors run in registration order by default. Implement `IOrderBehavior` to control the position —
a **lower `Order` runs first (outermost)**; behaviors without it default to `int.MaxValue` and run
closest to the handler:

```csharp
public sealed class AuditBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    public int Order => 10;
    // ...
}
```

The built-in exception behavior uses `int.MinValue` so it wraps everything; the timeout behavior uses
a low negative value so it sits just inside it.

## Make it free when idle — `IConditionalPipelineBehavior`

If your behavior only applies to some request types, implement `IConditionalPipelineBehavior`. When
`IsActive` is `false`, the executor omits the behavior entirely — no async frame, no delegate — so it
costs nothing for requests it doesn't apply to.

```csharp
public sealed class CacheBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    // Computed once per closed type; cheap to evaluate per request.
    private static readonly bool Applies = typeof(ICacheableRequest).IsAssignableFrom(typeof(TRequest));

    public bool IsActive => Applies;

    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        // Safe to assume the request applies — the executor only runs active behaviors.
        // ...
        return await handle();
    }
}
```

`IsActive` is evaluated per request, so it can also depend on runtime state (the built-in logging
behavior, for instance, is active only while information-level logging is enabled).

## Streaming behaviors

For `IStreamRequest<T>`, implement `IStreamPipelineBehavior<TRequest, TResponse>` instead; its
`Handle` returns and composes `IAsyncEnumerable<TResponse>` and receives a `Func<IAsyncEnumerable<TResponse>> continuation`.
