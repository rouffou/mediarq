using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Command;

/// <summary>
/// Represents a command request that encapsulates an intention to perform an action
/// which changes the system's state and produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <remarks>
/// A command expresses an operation that performs side effects — such as creating,
/// updating, or deleting data — within the system.  
/// 
/// In the CQRS (Command and Query Responsibility Segregation) pattern, commands are used
/// to **change** state, while queries are used to **read** state.  
/// 
/// Each <see cref="ICommand{TResponse}"/> should have a corresponding handler implementing
/// <see cref="IRequestHandler{TRequest, TResponse}"/> to execute the desired logic.
/// </remarks>
/// <typeparam name="TResponse">
/// The type of the response returned after the command is executed.
/// </typeparam>
/// <example>
/// <code>
/// public record CreateUserCommand(string Name, string Email) : ICommand&lt;Result&lt;UserDto&gt;&gt;;
///
/// public class CreateUserCommandHandler : IRequestHandler&lt;CreateUserCommand, Result&lt;UserDto&gt;&gt;
/// {
///     public async Task&lt;Result&lt;UserDto&gt;&gt; Handle(CreateUserCommand request, CancellationToken cancellationToken)
///     {
///         var user = new User { Name = request.Name, Email = request.Email };
///         // Save to database...
///         return Result.Success(new UserDto(user.Id, user.Name, user.Email));
///     }
/// }
/// </code>
/// </example>
public interface ICommand<TResponse> : ICommandOrQuery<TResponse>;

public interface ICommand : IRequest;
