using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Pipeline behavior that bounds how long a request annotated with <see cref="ITimeoutRequest"/> may
/// take. If the handler does not complete within the request's <see cref="ITimeoutRequest.Timeout"/>,
/// the behavior throws a <see cref="RequestTimeoutException"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
/// <remarks>
/// Opt in with <c>AddMediarqTimeout()</c>. The behavior is active only for request types implementing
/// <see cref="ITimeoutRequest"/>; for all others it is omitted from the pipeline. It runs near the
/// outside of the pipeline (just inside exception handling) so the whole inner pipeline is timed. The
/// timeout is pessimistic: it frees the caller, but the handler is not forcibly aborted — handlers
/// should also honor the <see cref="CancellationToken"/> they receive for true cooperative cancellation.
/// </remarks>
public sealed class TimeoutBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly bool RequestHasTimeout = typeof(ITimeoutRequest).IsAssignableFrom(typeof(TRequest));

    /// <summary>Active only for request types that implement <see cref="ITimeoutRequest"/>.</summary>
    public bool IsActive => RequestHasTimeout;

    /// <summary>Runs near the outermost position (just inside exception handling) so it bounds the whole inner pipeline.</summary>
    public int Order => -1000;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var timeout = ((ITimeoutRequest)context.Request!).Timeout;
        if (timeout <= TimeSpan.Zero)
        {
            // A non-positive timeout disables the behavior for this request.
            return await handle().ConfigureAwait(false);
        }

        using var timeoutCts = new CancellationTokenSource();
        var handlerTask = handle();
        var delayTask = Task.Delay(timeout, timeoutCts.Token);

        var completed = await Task.WhenAny(handlerTask, delayTask).ConfigureAwait(false);
        if (completed == handlerTask)
        {
            // The handler won the race: cancel the timer and surface the handler's result (or exception).
            await timeoutCts.CancelAsync().ConfigureAwait(false);
            return await handlerTask.ConfigureAwait(false);
        }

        throw new RequestTimeoutException(typeof(TRequest), timeout);
    }
}
