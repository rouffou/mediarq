using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Exceptions;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

/// <summary>
/// Pipeline behavior that catches exceptions thrown while handling a request and gives every
/// registered <see cref="IRequestExceptionHandler{TRequest, TResponse}"/> a chance to turn the
/// exception into a response (typically a failed <c>Result</c>). If none handle it, the original
/// exception is rethrown.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
/// <remarks>
/// It runs as the outermost behavior (<see cref="Order"/> = <see cref="int.MinValue"/>) so it can
/// observe exceptions from the whole pipeline and the handler. With no registered exception handler
/// it is a transparent pass-through.
/// </remarks>
public sealed class RequestExceptionProcessorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IEnumerable<IRequestExceptionHandler<TRequest, TResponse>> _exceptionHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestExceptionProcessorBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="exceptionHandlers">The exception handlers registered for this request type.</param>
    public RequestExceptionProcessorBehavior(IEnumerable<IRequestExceptionHandler<TRequest, TResponse>> exceptionHandlers)
    {
        _exceptionHandlers = exceptionHandlers;
    }

    /// <summary>Runs outermost so it can catch exceptions from every other behavior and the handler.</summary>
    public int Order => int.MinValue;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        try
        {
            return await handle().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            var state = new RequestExceptionHandlerState<TResponse>();

            foreach (var exceptionHandler in _exceptionHandlers)
            {
                await exceptionHandler.Handle(context.Request, exception, state, cancellationToken).ConfigureAwait(false);

                if (state.Handled)
                {
                    return state.Response!;
                }
            }

            throw;
        }
    }
}
