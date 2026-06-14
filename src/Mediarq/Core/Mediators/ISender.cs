using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Dispatches requests (commands and queries) to their handler. This is the request-sending half of
/// the mediator; inject it when a component only needs to send requests.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends a command or query to its handler and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The response type, typically a <see cref="Result"/> or <see cref="Result{T}"/>.</typeparam>
    /// <param name="request">The command or query implementing <see cref="ICommandOrQuery{TResponse}"/>.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <returns>A task producing the response of type <typeparamref name="TResponse"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    /// <exception cref="Mediarq.Core.Common.Exceptions.HandlerNotFoundException">Thrown when no handler is registered for the request.</exception>
    Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command that does not return a value. It flows through the same pipeline as other requests.
    /// </summary>
    /// <param name="request">The command to execute.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is <see langword="null"/>.</exception>
    Task Send(ICommand request, CancellationToken cancellationToken = default);
}
