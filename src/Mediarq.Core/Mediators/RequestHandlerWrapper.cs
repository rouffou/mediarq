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
    public abstract Task<TResponse> Handle(
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
    public override Task<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IRequestContextFactory requestContextFactory,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var handler = handlerResolver.Resolve<IRequestHandler<TRequest, TResponse>>()
            ?? throw new HandlerNotFoundException(typeof(TRequest));

        // Dispatch inline (no executor hop): resolve the behaviors, and when none are active for this
        // request type, invoke the handler directly — no request-context allocation, no delegate chain.
        var behaviors = handlerResolver.ResolveAll<IPipelineBehavior<TRequest, TResponse>>();
        var active = PipelineDispatch.SelectActive<TRequest, TResponse>(behaviors, out var activeCount);
        if (active is null)
        {
            return handler.Handle(typedRequest, cancellationToken);
        }

        RequestContext<TRequest, TResponse> context = requestContextFactory.Create<TRequest, TResponse>(typedRequest, cancellationToken);
        return PipelineDispatch.Run(active, activeCount, context, ct => handler.Handle(typedRequest, ct), cancellationToken);
    }
}
