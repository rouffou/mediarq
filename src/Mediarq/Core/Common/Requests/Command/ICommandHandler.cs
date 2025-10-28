using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Command;

/// <summary>
/// Defines a handler responsible for executing a specific <see cref="ICommand{TResponse}"/>.
/// </summary>
/// <remarks>
/// In the CQRS (Command and Query Responsibility Segregation) pattern, a command handler
/// encapsulates the logic required to perform an action that **modifies** the application's state.  
/// 
/// Each command handler is responsible for processing a single type of command and returning
/// a response of type <typeparamref name="TResponse"/>.  
/// 
/// This interface extends <see cref="IRequestHandler{TRequest, TResponse}"/>, ensuring that command handlers
/// integrate seamlessly into the Mediarq pipeline and benefit from cross-cutting behaviors such as
/// validation, logging, and performance tracking.
/// </remarks>
/// <typeparam name="TRequest">
/// The type of the command being handled. Must implement <see cref="ICommand{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned after command execution.
/// </typeparam>
/// <example>
/// <code>
/// public record UpdateUserEmailCommand(Guid UserId, string NewEmail) : ICommand&lt;Result&gt;;
///
/// public class UpdateUserEmailCommandHandler : ICommandHandler&lt;UpdateUserEmailCommand, Result&gt;
/// {
///     public async Task&lt;Result&gt; Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
///     {
///         // Example: perform an update in the data store
///         var user = await _userRepository.GetByIdAsync(request.UserId);
///         if (user == null)
///             return Result.Failure("User not found");
///
///         user.Email = request.NewEmail;
///         await _userRepository.SaveChangesAsync();
///
///         return Result.Success();
///     }
/// }
/// </code>
/// </example>
public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : ICommand<TResponse>;

public interface ICommandHandler<in TRequest> : IRequestHandler<TRequest>
    where TRequest : ICommand;
