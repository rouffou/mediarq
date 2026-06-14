using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Requests.Exceptions;

/// <summary>
/// Handles an exception thrown while processing a request, optionally turning it into a successful
/// pipeline outcome (typically a failed <c>Result</c>), instead of letting the exception propagate.
/// </summary>
/// <typeparam name="TRequest">The request type whose handling may throw.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
/// <remarks>
/// All registered handlers for a request are invoked in turn until one marks the exception as handled
/// via <see cref="RequestExceptionHandlerState{TResponse}.SetHandled"/>. If none handle it, the original
/// exception is rethrown. This aligns with the railway-oriented design: convert an exception into a
/// failed <c>Result</c> rather than throwing.
/// </remarks>
public interface IRequestExceptionHandler<in TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    /// <summary>
    /// Inspects <paramref name="exception"/> and, if it can be handled, sets the replacement response
    /// on <paramref name="state"/>.
    /// </summary>
    /// <param name="request">The request being processed when the exception was thrown.</param>
    /// <param name="exception">The exception thrown by the handler or a downstream behavior.</param>
    /// <param name="state">The state used to provide a replacement response and mark the exception handled.</param>
    /// <param name="cancellationToken">A token to observe while waiting for completion.</param>
    /// <returns>A task that completes when the handler has finished inspecting the exception.</returns>
    Task Handle(
        TRequest request,
        Exception exception,
        RequestExceptionHandlerState<TResponse> state,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Carries the outcome of <see cref="IRequestExceptionHandler{TRequest, TResponse}"/> invocation:
/// whether the exception was handled and, if so, the replacement response.
/// </summary>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class RequestExceptionHandlerState<TResponse>
{
    /// <summary>Gets a value indicating whether a handler has handled the exception.</summary>
    public bool Handled { get; private set; }

    /// <summary>Gets the replacement response set by a handler, valid only when <see cref="Handled"/> is <see langword="true"/>.</summary>
    public TResponse? Response { get; private set; }

    /// <summary>
    /// Marks the exception as handled and supplies the response to return from the pipeline.
    /// </summary>
    /// <param name="response">The response to return instead of throwing.</param>
    public void SetHandled(TResponse response)
    {
        Handled = true;
        Response = response;
    }
}
