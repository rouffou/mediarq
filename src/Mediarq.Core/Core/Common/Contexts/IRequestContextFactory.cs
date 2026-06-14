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
    /// Creates a strongly-typed <see cref="RequestContext{TRequest, TResponse}"/> for the specified command or query.
    /// </summary>
    /// <typeparam name="TRequest">The type of the command or query. Must implement <see cref="ICommandOrQuery{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the result produced by the command or query.</typeparam>
    /// <param name="request">The command or query instance. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="RequestContext{TRequest, TResponse}"/> describing the request and its execution environment.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="request"/> is null.</exception>
    RequestContext<TRequest, TResponse> Create<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>;
}
