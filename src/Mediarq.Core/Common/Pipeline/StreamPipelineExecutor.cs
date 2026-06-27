using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>Builds and runs the chain of <see cref="IStreamPipelineBehavior{TRequest, TResponse}"/> around a stream handler.</summary>
public interface IStreamPipelineExecutor
{
    /// <summary>Wraps <paramref name="handler"/> with the registered stream behaviors and returns the resulting stream.</summary>
    IAsyncEnumerable<TResponse> Execute<TRequest, TResponse>(TRequest request, Func<IAsyncEnumerable<TResponse>> handler, CancellationToken cancellationToken)
        where TRequest : IStreamRequest<TResponse>;
}

/// <summary>
/// Default <see cref="IStreamPipelineExecutor"/>: resolves every
/// <see cref="IStreamPipelineBehavior{TRequest, TResponse}"/>, orders them by <see cref="IOrderBehavior"/>
/// (lower first/outermost) and wraps them around the handler — mirroring <see cref="PipelineExecutor"/>.
/// </summary>
public class StreamPipelineExecutor(IHandlerResolver handlerResolver) : IStreamPipelineExecutor
{
    private readonly IHandlerResolver _handlerResolver = handlerResolver;

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> Execute<TRequest, TResponse>(TRequest request, Func<IAsyncEnumerable<TResponse>> handler, CancellationToken cancellationToken)
        where TRequest : IStreamRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(handler);

        IReadOnlyList<IStreamPipelineBehavior<TRequest, TResponse>> behaviors =
            _handlerResolver.ResolveAll<IStreamPipelineBehavior<TRequest, TResponse>>();

        var orderedBehaviors = behaviors.OrderBy(b => b is IOrderBehavior order ? order.Order : int.MaxValue);

        Func<IAsyncEnumerable<TResponse>> next = handler;
        foreach (var behavior in orderedBehaviors.Reverse())
        {
            var currentNext = next;
            var currentBehavior = behavior;
            next = () => currentBehavior.Handle(request, currentNext, cancellationToken);
        }

        return next();
    }
}
