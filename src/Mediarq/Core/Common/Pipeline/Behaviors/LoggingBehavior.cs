using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.Logging;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Represents a pipeline behavior that logs request execution details,
/// including when a request starts and when it completes.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being processed.
/// Must implement <see cref="ICommandOrQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the request handler.
/// </typeparam>
/// <remarks>
/// This behavior is typically registered as part of the Mediarq pipeline.
/// It logs structured information before and after a request is executed,
/// such as the request type, unique identifier, and execution timestamps.  
/// Useful for monitoring request flow and debugging distributed systems.
/// </remarks>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger used to record request lifecycle events.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="logger"/> is <see langword="null"/>.
    /// </exception>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Intercepts the execution of a request to log start and completion details.
    /// </summary>
    /// <param name="context">
    /// The current request context containing metadata such as the request ID and timestamps.
    /// </param>
    /// <param name="handle">
    /// The delegate representing the next step in the pipeline or the final request handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to propagate cancellation signals.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, producing the response (<typeparamref name="TResponse"/>).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="context"/> or <paramref name="handle"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// The behavior performs the following actions:
    /// <list type="number">
    ///   <item><description>Logs the beginning of request handling with timestamp and request ID.</description></item>
    ///   <item><description>Executes the next delegate in the pipeline (which may be another behavior or the request handler).</description></item>
    ///   <item><description>Logs completion details after the request has been handled.</description></item>
    /// </list>
    /// This helps trace requests across multiple layers and improves observability in production environments.
    /// </remarks>
    public Task<TResponse> Handle(IIMMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        _logger.LogInformation("Handling {RequestType} with RequestId {RequestId} started at {StartedAt}", typeof(TRequest).Name, context.RequestId, context.StartedAt);

        Task<TResponse> response = handle();

        _logger.LogInformation("Handled {RequestType} with RequestId {RequestId} ended at {EndedAt}", typeof(TRequest).Name, context.RequestId, context.FinishedAt);

        return response;
    }
}
