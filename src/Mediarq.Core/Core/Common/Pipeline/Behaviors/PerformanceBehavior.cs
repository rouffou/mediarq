using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Time;
using Microsoft.Extensions.Logging;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Represents a pipeline behavior responsible for measuring and logging
/// the execution time of requests handled through the <see cref="ICommandOrQuery{TResponse}"/> pattern.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being processed.
/// Must implement <see cref="ICommandOrQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the request handler.
/// </typeparam>
/// <remarks>
/// This behavior measures the total execution time of a request handler.
/// If the duration exceeds a predefined threshold (default: 500 ms),
/// it logs a warning message.
/// Timing is measured through the injected <see cref="IClock"/> abstraction so the
/// behavior remains deterministically testable.
/// </remarks>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private const long ThresholdMilliseconds = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger used to record execution times and performance warnings.
    /// </param>
    /// <param name="clock">
    /// The clock abstraction used for timing operations and improving testability.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="logger"/> or <paramref name="clock"/> is <see langword="null"/>.
    /// </exception>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(clock);

        _logger = logger;
        _clock = clock;
    }

    /// <summary>
    /// Intercepts the request handling process to measure execution time
    /// and logs a warning if it exceeds a defined performance threshold.
    /// </summary>
    /// <param name="context">
    /// The current request context containing metadata such as request ID and timestamps.
    /// </param>
    /// <param name="handle">
    /// The delegate representing the next step in the pipeline or the final request handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, producing the response (<typeparamref name="TResponse"/>).
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="context"/> or <paramref name="handle"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// The behavior performs the following steps:
    /// <list type="number">
    ///   <item><description>Reads the current time from <see cref="IClock"/> before invoking the next delegate.</description></item>
    ///   <item><description>Executes the request handler or subsequent behaviors.</description></item>
    ///   <item><description>Reads the time again and logs a warning if execution exceeded 500 milliseconds.</description></item>
    /// </list>
    /// </remarks>
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        DateTime start = _clock.UtcNow;
        TResponse response = await handle();
        long elapsedMs = (long)(_clock.UtcNow - start).TotalMilliseconds;

        if (elapsedMs > ThresholdMilliseconds)
        {
            _logger.LogWarning("Long running request: {RequestName} ({ElapsedMilliseconds} ms)", typeof(TRequest).Name, elapsedMs);
        }

        return response;
    }
}
