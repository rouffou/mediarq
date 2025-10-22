using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Defines a contract for executing a request pipeline composed of one or more <see cref="IPipelineBehavior{TRequest, TResponse}"/> instances.
/// </summary>
/// <remarks>
/// The <see cref="IPipelineExecutor"/> is responsible for orchestrating the sequential execution
/// of all registered pipeline behaviors before invoking the actual request handler.  
/// 
/// Each behavior can inspect, modify, or short-circuit the request processing flow.  
/// This abstraction allows Mediarq to support cross-cutting concerns (e.g., validation, logging, performance)
/// in a consistent and composable manner.
/// </remarks>
public interface IPipelineExecutor
{
    /// <summary>
    /// Executes the request pipeline for a given request and response type.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The type of the request to be handled.
    /// Must implement <see cref="ICommandOrQuery{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response returned after the request is processed.
    /// </typeparam>
    /// <param name="context">
    /// The current <see cref="RequestContext{TRequest, TResponse}"/> containing metadata,
    /// execution details, and the request itself.
    /// </param>
    /// <param name="handlerDelegate">
    /// The final delegate representing the actual request handler to be executed
    /// once all pipeline behaviors have been invoked.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete, allowing for cancellation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous execution of the pipeline that produces
    /// a response of type <typeparamref name="TResponse"/>.
    /// </returns>
    /// <remarks>
    /// The typical execution flow is as follows:
    /// <list type="number">
    ///   <item><description>The executor resolves all <see cref="IPipelineBehavior{TRequest, TResponse}"/> implementations.</description></item>
    ///   <item><description>Behaviors are invoked in the order they are registered, forming a middleware-like chain.</description></item>
    ///   <item><description>Each behavior calls <c>next()</c> to continue execution or stops the chain to return early.</description></item>
    ///   <item><description>Finally, the actual handler is invoked to process the request and produce the response.</description></item>
    /// </list>
    /// 
    /// Example:
    /// <code>
    /// var response = await pipelineExecutor.ExecuteAsync(
    ///     context,
    ///     ct => handler.Handle(request, ct),
    ///     cancellationToken);
    /// </code>
    /// 
    /// Implementations must ensure correct ordering and safe execution of all behaviors.
    /// </remarks>
    Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        RequestContext<TRequest, TResponse> context,
        Func<CancellationToken, Task<TResponse>> handlerDelegate,
        CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>;
}
