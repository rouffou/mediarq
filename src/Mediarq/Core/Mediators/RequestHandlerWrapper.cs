using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Generic over the response type, exposing the entry point the <see cref="Mediator"/> calls.
/// Implementations are cached (as <see cref="object"/>) per concrete request type.
/// </summary>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
internal interface IRequestHandlerWrapper<TResponse>
{
    Task<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IRequestContextFactory requestContextFactory,
        IPipelineExecutor pipelineExecutor,
        CancellationToken cancellationToken);
}

/// <summary>
/// Closed over both the concrete request type and the response type. Because both type arguments
/// are known at this point, the handler resolution, context creation and pipeline execution are all
/// strongly-typed — no <c>dynamic</c> and no per-call reflection on the mediator hot path.
/// </summary>
/// <typeparam name="TRequest">The concrete request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
internal sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : IRequestHandlerWrapper<TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    public Task<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IRequestContextFactory requestContextFactory,
        IPipelineExecutor pipelineExecutor,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var handler = handlerResolver.Resolve(typeof(IRequestHandler<TRequest, TResponse>)) as IRequestHandler<TRequest, TResponse>
            ?? throw new HandlerNotFoundException(typeof(TRequest));

        RequestContext<TRequest, TResponse> context = requestContextFactory.Create<TRequest, TResponse>(typedRequest, cancellationToken);

        return pipelineExecutor.ExecuteAsync(
            context,
            ct => handler.Handle(typedRequest, ct),
            cancellationToken);
    }
}
