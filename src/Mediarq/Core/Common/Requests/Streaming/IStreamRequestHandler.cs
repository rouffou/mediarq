namespace Mediarq.Core.Common.Requests.Streaming;

/// <summary>
/// Handles an <see cref="IStreamRequest{TResponse}"/>, producing a stream of
/// <typeparamref name="TResponse"/> items.
/// </summary>
/// <typeparam name="TRequest">The stream request type.</typeparam>
/// <typeparam name="TResponse">The type of each streamed item.</typeparam>
public interface IStreamRequestHandler<in TRequest, out TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    /// <summary>
    /// Handles the <paramref name="request"/> and streams the resulting items.
    /// </summary>
    /// <param name="request">The stream request to handle.</param>
    /// <param name="cancellationToken">A token to observe while streaming.</param>
    /// <returns>An asynchronous stream of <typeparamref name="TResponse"/> items.</returns>
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
