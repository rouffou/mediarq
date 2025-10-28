using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Defines a mediator responsible for dispatching requests (commands or queries)
/// to their corresponding handlers within the application.
/// </summary>
/// <remarks>
/// The mediator acts as the central entry point for executing application requests.
/// It decouples the sending component (e.g., controllers, services) from the
/// handling component (command/query handlers).
///
/// This interface supports asynchronous execution of requests and integrates 
/// seamlessly with pipeline behaviors such as logging, validation, and performance tracking.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// public class CreateUserCommand : ICommand&lt;Result&lt;User&gt;&gt;
/// {
///     public string UserName { get; set; }
/// }
///
/// public class CreateUserHandler : ICommandHandler&lt;CreateUserCommand, Result&lt;User&gt;&gt;
/// {
///     public Task&lt;Result&lt;User&gt;&gt; Handle(CreateUserCommand request, CancellationToken cancellationToken)
///     {
///         var user = new User { Name = request.UserName };
///         return Task.FromResult(Result.Success(user));
///     }
/// }
///
/// // Using the mediator
/// var result = await _mediator.Send(new CreateUserCommand { UserName = "Alice" });
/// </code>
/// </example>
public interface IMediator
{
    /// <summary>
    /// Sends a request (command or query) to its appropriate handler for processing.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of response returned by the handler. 
    /// Typically, this is a <see cref="Result"/> or <see cref="Result{T}"/> instance.
    /// </typeparam>
    /// <param name="request">
    /// The request instance implementing <see cref="ICommandOrQuery{TResponse}"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the response of type <typeparamref name="TResponse"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no matching handler is found for the given request type.
    /// </exception>
    Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a command without expecting a result (for side effects).
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Send(ICommand request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all registered handlers.
    /// </summary>
    /// <param name="notification">The notification instance.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
}
