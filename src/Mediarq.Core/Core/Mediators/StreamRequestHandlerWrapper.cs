using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Generic over the streamed item type, exposing the entry point the <see cref="Mediator"/> calls to
/// stream a request. Implementations are cached per concrete stream-request type.
/// </summary>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
internal interface IStreamRequestHandlerWrapper<out TResponse>
{
    IAsyncEnumerable<TResponse> Handle(
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
internal sealed class StreamRequestHandlerWrapperImpl<TRequest, TResponse> : IStreamRequestHandlerWrapper<TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    public IAsyncEnumerable<TResponse> Handle(
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
