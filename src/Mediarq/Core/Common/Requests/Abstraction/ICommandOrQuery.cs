namespace Mediarq.Core.Common.Requests.Abstraction;

/// <summary>
/// Defines a common abstraction for both commands and queries that expect a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <remarks>
/// This interface serves as a unifying contract between <c>ICommand&lt;TResponse&gt;</c> and <c>IQuery&lt;TResponse&gt;</c>.
/// It enables consistent handling of request messages within the mediator pipeline,
/// regardless of whether they represent a command (write operation) or a query (read operation).
/// </remarks>
/// <typeparam name="TResponse">
/// The type of the response returned after the request is processed.
/// </typeparam>
/// <example>
/// <code>
/// public class CreateUserCommand : ICommandOrQuery<Result<UserDto>>
/// {
///     public string Name { get; }
///     public string Email { get; }
///
///     public CreateUserCommand(string name, string email)
///     {
///         Name = name;
///         Email = email;
///     }
/// }
/// </code>
/// </example>

public interface ICommandOrQuery<TResponse> : IRequest<TResponse>;

