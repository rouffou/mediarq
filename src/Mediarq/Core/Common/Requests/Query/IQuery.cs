using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Query;

/// <summary>
/// Represents a request that retrieves data without modifying the application's state.
/// </summary>
/// <remarks>
/// In the CQRS (Command and Query Responsibility Segregation) pattern,  
/// a <see cref="IQuery{TResponse}"/> defines a read-only operation — it should **not** produce any side effects.  
/// 
/// Queries are typically handled by an <c>IQueryHandler&lt;TQuery, TResponse&gt;</c>,  
/// which executes data retrieval logic (for example, database reads, projections, or API calls)
/// and returns a result of type <typeparamref name="TResponse"/>.
/// 
/// This interface extends <see cref="ICommandOrQuery{TResponse}"/>,  
/// making it compatible with the Mediarq request pipeline and its behaviors (e.g., logging, validation, performance monitoring).
/// </remarks>
/// <typeparam name="TResponse">
/// The type of the result returned when executing the query.
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
public interface IQuery<TResponse> : ICommandOrQuery<TResponse>;
