using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Query;

/// <summary>
/// Defines a handler responsible for executing a specific <see cref="IQuery{TResponse}"/>.
/// </summary>
/// <remarks>
/// In the CQRS (Command and Query Responsibility Segregation) pattern, a query handler
/// encapsulates the logic required to perform an action that **read** the application's state.  
/// 
/// Each query handler is responsible for processing a single type of query and returning
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
/// </typeparam>
/// <example>
/// <code>
/// public record GetUserByIdQuery(Guid UserId) : IQuery&lt;UserDto&gt;;
///
/// public class GetUserByIdQueryHandler : IQueryHandler&lt;GetUserByIdQuery, UserDto&gt;
/// {
///     public async Task&lt;UserDto&gt; Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
///     {
///         var user = await _userRepository.GetByIdAsync(request.UserId);
///         return user is null ? null : new UserDto(user.Id, user.Name, user.Email);
///     }
/// }
/// </code>
/// </example>
public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IQuery<TResponse>;

