using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Contexts;

/// <summary>
/// Defines a factory for creating request context objects for command or query operations.
/// </summary>
/// <remarks>Implementations of this interface are responsible for generating context objects that encapsulate
/// information about a specific request and its execution environment. The context object can be used to manage
/// request-scoped data, such as cancellation tokens or metadata, throughout the processing of the request.</remarks>
public interface IRequestContextFactory
{
    /// <summary>
    /// Creates and executes the specified command or query, returning the result as an object.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command or query to execute. Must implement ICommandOrQuery<TResponse>.</typeparam>
    /// <typeparam name="TResponse">The type of the result produced by the command or query. The type of the response should be <see cref="Results.Result"/> or <see cref="Results.Result{TValue}"/></typeparam>
    /// <param name="request">The command or query instance to execute. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An object containing the result of the command or query execution. The returned object is of type TResponse.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="request"/> is null.</exception>
    object Create<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>;
}
