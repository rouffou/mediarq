namespace Mediarq.Core.Common.Requests.Processors;

/// <summary>
/// Runs before the handler for <typeparamref name="TRequest"/>. All registered pre-processors run,
/// in registration order, before the request reaches its handler.
/// </summary>
/// <typeparam name="TRequest">The request type to pre-process.</typeparam>
public interface IRequestPreProcessor<in TRequest>
{
    /// <summary>Performs work before the request is handled.</summary>
    /// <param name="request">The request about to be handled.</param>
    /// <param name="cancellationToken">A token to observe while processing.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Process(TRequest request, CancellationToken cancellationToken = default);
}
