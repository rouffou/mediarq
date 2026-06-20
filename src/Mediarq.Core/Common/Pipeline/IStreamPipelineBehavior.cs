using Mediarq.Core.Common.Requests.Streaming;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Cross-cutting behavior that wraps a streaming request, analogous to
/// <see cref="IPipelineBehavior{TRequest, TResponse}"/> for <c>Send</c>. Implement
/// <see cref="IOrderBehavior"/> to control ordering (lower runs first/outermost).
/// </summary>
/// <typeparam name="TRequest">The stream-request type.</typeparam>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
public interface IStreamPipelineBehavior<in TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    /// <summary>
    /// Wraps the rest of the stream pipeline. Call <paramref name="continuation"/> to obtain the
    /// downstream sequence; you may observe, transform, or short-circuit it.
    /// </summary>
    /// <param name="request">The stream request being handled.</param>
    /// <param name="continuation">Produces the next stream in the pipeline (ultimately the handler).</param>
    /// <param name="cancellationToken">A token to observe while streaming.</param>
    /// <returns>The (possibly wrapped) stream of <typeparamref name="TResponse"/> items.</returns>
    IAsyncEnumerable<TResponse> Handle(TRequest request, Func<IAsyncEnumerable<TResponse>> continuation, CancellationToken cancellationToken);
}
