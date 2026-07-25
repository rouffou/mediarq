using System.Diagnostics.CodeAnalysis;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Generic over the response type, exposing the entry point the <see cref="Mediator"/> calls. An
/// abstract class (rather than an interface) so the call dispatches through a vtable slot, which is a
/// little cheaper than interface dispatch on this hot path. Implementations are cached (as
/// <see cref="object"/>) per concrete request type.
/// </summary>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
[SuppressMessage("Major Code Smell", "S1694:An abstract class should have both abstract and concrete methods",
    Justification = "Intentionally an abstract class, not an interface: vtable dispatch is cheaper than interface dispatch on the mediator hot path.")]
internal abstract class RequestHandlerWrapper<TResponse>
{
    // Internal-only: returns ValueTask<TResponse> rather than Task<TResponse> so the common
    // "known zero behaviors" fast path (see PipelineDispatch.ExecuteWithBehaviorCache) never wraps an
    // already-existing Task<TResponse> in another one. Mediator.Send (the public, Task<TResponse>-returning
    // boundary) converts once via ValueTask<TResponse>.AsTask(), which is itself allocation-free when the
    // ValueTask already wraps a real Task<TResponse> — always true here, since nothing on this path ever
    // constructs a ValueTask<TResponse> from a bare value.
    public abstract ValueTask<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IRequestContextFactory requestContextFactory,
        CancellationToken cancellationToken);
}

/// <summary>
/// Closed over both the concrete request type and the response type. Because both type arguments
/// are known at this point, the handler resolution, context creation and pipeline execution are all
/// strongly-typed — no <c>dynamic</c> and no per-call reflection on the mediator hot path.
/// </summary>
/// <typeparam name="TRequest">The concrete request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
internal sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    public override ValueTask<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IRequestContextFactory requestContextFactory,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var handler = handlerResolver.Resolve<IRequestHandler<TRequest, TResponse>>()
            ?? throw new HandlerNotFoundException(typeof(TRequest));

        // Same cache-aware dispatch as PipelineExecutor's handler overload (shared via PipelineDispatch),
        // so a request type observed to have zero registered behaviors skips ResolveAll<IPipelineBehavior<,>>
        // entirely on every subsequent call, not just the delegate/context allocation.
        var behaviorRegistrationCache = handlerResolver.Resolve<PipelineBehaviorRegistrationCache>();
        return PipelineDispatch.ExecuteWithBehaviorCache(
            handlerResolver, behaviorRegistrationCache, typedRequest, handler, requestContextFactory, cancellationToken);
    }
}
