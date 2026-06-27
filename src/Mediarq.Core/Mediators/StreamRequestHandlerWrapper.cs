using System.Diagnostics.CodeAnalysis;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Generic over the streamed item type, exposing the entry point the <see cref="Mediator"/> calls to
/// stream a request. An abstract class (rather than an interface) so the call dispatches through a
/// vtable slot. Implementations are cached per concrete stream-request type.
/// </summary>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
[SuppressMessage("Major Code Smell", "S1694:An abstract class should have both abstract and concrete methods",
    Justification = "Intentionally an abstract class, not an interface: vtable dispatch is cheaper than interface dispatch on the streaming hot path.")]
internal abstract class StreamRequestHandlerWrapper<TResponse>
{
    public abstract IAsyncEnumerable<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IStreamPipelineExecutor? pipelineExecutor,
        CancellationToken cancellationToken);
}

/// <summary>
/// Closed over the concrete request and item types, so handler resolution and invocation are
/// strongly-typed — no reflection on the streaming dispatch path. Runs the handler through the stream
/// pipeline when an executor is supplied.
/// </summary>
/// <typeparam name="TRequest">The concrete stream-request type.</typeparam>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
internal sealed class StreamRequestHandlerWrapperImpl<TRequest, TResponse> : StreamRequestHandlerWrapper<TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    public override IAsyncEnumerable<TResponse> Handle(
        object request,
        IHandlerResolver handlerResolver,
        IStreamPipelineExecutor? pipelineExecutor,
        CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var handler = handlerResolver.Resolve<IStreamRequestHandler<TRequest, TResponse>>()
            ?? throw new HandlerNotFoundException(typeof(TRequest));

        IAsyncEnumerable<TResponse> Stream() => handler.Handle(typedRequest, cancellationToken);

        return pipelineExecutor is null
            ? Stream()
            : pipelineExecutor.Execute<TRequest, TResponse>(typedRequest, Stream, cancellationToken);
    }
}
