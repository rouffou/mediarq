namespace Mediarq.Core.Common.Requests.Abstraction;

/// <summary>
/// Defines the contract for handling a specific request of type <typeparamref name="TRequest"/>
/// and producing a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <remarks>
/// This interface represents the core of the mediator pattern — it encapsulates a single unit of work
/// associated with a specific command or query. Each request type should have exactly one corresponding
/// handler implementation that contains the application logic to process the request.
/// 
/// The <typeparamref name="TRequest"/> must implement <see cref="ICommandOrQuery{TResponse}"/>,
/// ensuring consistency across the mediator pipeline and type safety in the request–response mapping.
/// </remarks>
/// <typeparam name="TRequest">
/// The type of the request to handle. Must implement <see cref="ICommandOrQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned after handling the request.
/// </typeparam>
/// <example>
/// <code>
/// public class CreateUserCommandHandler : IRequestHandler&lt;CreateUserCommand, Result&lt;UserDto&gt;&gt;
/// {
///     public async Task&lt;Result&lt;UserDto&gt;&gt; Handle(CreateUserCommand request, CancellationToken cancellationToken)
///     {
///         var user = new User { Name = request.Name, Email = request.Email };
///         // Persist user to the database...
///         return Result.Success(new UserDto(user.Id, user.Name, user.Email));
///     }
/// }
/// </code>
/// </example>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    /// <summary>
    /// Handles the specified <paramref name="request"/> asynchronously and produces a response.
    /// </summary>
    /// <param name="request">The request message containing all data required for processing.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the <typeparamref name="TResponse"/> result.
    /// </returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}


/// <summary>
/// Defines a handler for a request of type <typeparamref name="TRequest"/> that does not return a value.
/// </summary>
/// <typeparam name="TRequest">
/// The request type. Its response type is <see cref="Unit"/>, allowing it to share the request pipeline.
/// </typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : ICommandOrQuery<Unit>
{
    /// <summary>
    /// Handles the specified <paramref name="request"/> asynchronously.
    /// </summary>
    /// <param name="request">The request message containing all data required for processing.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation.
    /// </returns>
    new Task Handle(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adapts the no-result handler to the <see cref="IRequestHandler{TRequest, TResponse}"/> contract,
    /// returning <see cref="Unit.Value"/> after the request has been handled. This lets commands without
    /// a return value reuse the same pipeline as commands and queries that produce a response.
    /// </summary>
    /// <param name="request">The request message containing all data required for processing.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes with <see cref="Unit.Value"/>.</returns>
    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await ((IRequestHandler<TRequest>)this).Handle(request, cancellationToken);
        return Unit.Value;
    }
}
