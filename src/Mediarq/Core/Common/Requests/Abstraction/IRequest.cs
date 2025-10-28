using System.ComponentModel;

namespace Mediarq.Core.Common.Requests.Abstraction;

[EditorBrowsable(EditorBrowsableState.Never)]
/// <summary>
/// Represents a marker interface for defining a request that expects a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <remarks>
/// This interface is used to identify objects that can be processed by a mediator or request handler.
/// Implementations typically encapsulate input data required to execute a specific operation or query.
/// </remarks>
/// <typeparam name="TResponse">
/// The type of the response returned after the request is handled.
/// </typeparam>
/// <example>
/// <code>
/// public class GetUserByIdQuery : IRequest<UserDto>
/// {
///     public Guid UserId { get; }
///
///     public GetUserByIdQuery(Guid userId)
///     {
///         UserId = userId;
///     }
/// }
/// </code>
/// </example>

public interface IRequest<TResponse>;


[EditorBrowsable(EditorBrowsableState.Never)]
public interface IRequest;
