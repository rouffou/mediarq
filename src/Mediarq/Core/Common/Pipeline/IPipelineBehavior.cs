using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Defines a behavior that can be executed within the Mediarq request handling pipeline.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being handled.
/// Must implement <see cref="ICommandOrQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the request handler.
/// </typeparam>
/// <remarks>
/// A pipeline behavior allows developers to insert cross-cutting logic that executes
/// before or after the request handler.  
/// Common examples include:
/// <list type="bullet">
///   <item><description>Validation of the request data.</description></item>
///   <item><description>Logging request and response information.</description></item>
///   <item><description>Performance or timing measurement.</description></item>
///   <item><description>Exception handling or retry policies.</description></item>
/// </list>
/// 
/// Each behavior receives a reference to the <paramref name="next"/> delegate, which represents
/// the next step in the pipeline. Calling <paramref name="next"/> continues the pipeline execution;
/// omitting it short-circuits the request handling process.
/// </remarks>
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    /// <summary>
    /// Handles the request within the pipeline by optionally executing logic
    /// before and/or after calling the next delegate.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="IIMMutableRequestContext{TRequest, TResponse}"/> containing
    /// the request data, metadata, and contextual information.
    /// </param>
    /// <param name="next">
    /// A delegate representing the next behavior or the final handler to execute.
    /// The implementer must call this delegate to continue the pipeline.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to propagate cancellation signals across asynchronous operations.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation that produces the response (<typeparamref name="TResponse"/>).
    /// </returns>
    /// <remarks>
    /// Implementations of this method are typically short-lived and stateless.  
    /// They should handle exceptions gracefully and avoid blocking asynchronous execution.  
    /// 
    /// Example behaviors:
    /// <list type="bullet">
    ///   <item><description><c>LoggingBehavior</c> – logs request start and completion.</description></item>
    ///   <item><description><c>ValidationBehavior</c> – validates the request before passing it to the next handler.</description></item>
    ///   <item><description><c>PerformanceBehavior</c> – measures execution time and logs slow requests.</description></item>
    /// </list>
    /// </remarks>
    Task<TResponse> Handle(IIMMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default);        
}
