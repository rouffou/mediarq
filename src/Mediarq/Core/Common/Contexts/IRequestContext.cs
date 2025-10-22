using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Contexts;

/// <summary>
/// Represent a request context. That contains information about the current request being processed.
/// </summary>
/// <typeparam name="TRequest">The type of the request that need to extends <see cref="ICommandOrQuery{TResponse}"/> interface.</typeparam>
/// <typeparam name="TResponse">The type of the responses must be <see cref="Results.Result"/> or <see cref="Results.Result{TValue}"/>.</typeparam>
public interface IRequestContext<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    /// <summary>
    /// The request unique identifier that allow to identify the request during the processing.
    /// </summary>
    Guid RequestId { get; }

    /// <summary>
    /// Gets the unique identifier used to correlate related operations or requests.
    /// </summary>
    /// <remarks>Use this identifier to track and associate logs, messages, or activities that are part of the
    /// same logical operation across system boundaries.</remarks>
    Guid CorrelationId { get; }

    /// <summary>
    /// The user unique identifier that is processing the request. Allow to identify who want to perform the operation.
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// The date and time when the request processing started.
    /// </summary>
    DateTime StartedAt { get; }

    /// <summary>
    /// The date and time when the request processing finished.
    /// </summary>
    DateTime FinishedAt { get; set; }

    /// <summary>
    /// The request being processed.
    /// </summary>
    TRequest Request { get; }

    /// <summary>
    /// Gets the <see cref="CancellationToken"/> that is used to observe cancellation requests for the current
    /// operation.
    /// </summary>
    /// <remarks>Use this token to monitor for cancellation requests and to cooperatively cancel ongoing work.
    /// The token may be used to register callbacks or to throw an <see cref="OperationCanceledException"/> when
    /// cancellation is requested.</remarks>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets a read-only collection of key/value pairs that can be used to share data within the current context.
    /// </summary>
    /// <remarks>The items in this dictionary are intended for storing and retrieving arbitrary data
    /// associated with the current operation or request. Keys are case-sensitive. Modifications to the returned
    /// dictionary are not permitted; attempting to do so will result in a runtime exception.</remarks>
    IReadOnlyDictionary<string, object> Items { get; }
}
