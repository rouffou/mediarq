namespace Mediarq.Core.Common.Requests.Processors;

/// <summary>
/// Runs after the handler for <typeparamref name="TRequest"/>. All registered post-processors run,
/// in registration order, once the handler has produced its <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">The request type that was handled.</typeparam>
/// <typeparam name="TResponse">The response produced by the handler.</typeparam>
public interface IRequestPostProcessor<in TRequest, in TResponse>
{
    /// <summary>Performs work after the request has been handled.</summary>
    /// <param name="request">The request that was handled.</param>
    /// <param name="response">The response produced by the handler.</param>
    /// <param name="cancellationToken">A token to observe while processing.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Process(TRequest request, TResponse response, CancellationToken cancellationToken = default);
}
