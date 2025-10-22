using Mediarq.Core.Common.Requests.Abstraction;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Mediarq.Core.Common.Contexts;

/// <summary>
/// Represents the context for a request, including metadata, timing, user information, and custom items associated with
/// the request lifecycle.
/// </summary>
/// <remarks>The <see cref="RequestContext{TRequest, TResponse}"/> class provides a container for request-specific
/// data, such as correlation and request identifiers, user information, timing, and cancellation support. It also
/// allows storing and retrieving custom items relevant to the request. This context is typically used to pass
/// information through the processing pipeline of a command or query. The class is not thread-safe for concurrent
/// modifications to the items collection.</remarks>
/// <typeparam name="TRequest">The type of the request being processed. Must implement <see cref="ICommandOrQuery{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response expected from the request. The response should be <see cref="Results.Result"/> or <see cref="Results.Result{TValue}"/> type.</typeparam>
[DebuggerDisplay("RequestContext {RequestId} ({typeof(TRequest).Name})")]
public record RequestContext<TRequest, TResponse>
    : IIMMutableRequestContext<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IDictionary<string, object> _items = new Dictionary<string, object>();

    /// <summary>
    /// Gets the unique identifier for the request.
    /// </summary>
    public Guid RequestId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the unique identifier used to correlate related operations or requests.
    /// </summary>
    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the unique identifier of the user associated with this instance.
    /// </summary>
    public string UserId { get; init; }

    /// <summary>
    /// Gets the UTC date and time when the operation started.
    /// </summary>
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the date and time when the operation was completed.
    /// </summary>
    public DateTime FinishedAt { get; set; }

    /// <summary>
    /// Gets the request associated with the operation.
    /// </summary>
    public TRequest Request { get; init; }

    /// <summary>
    /// Gets the <see cref="CancellationToken"/> that is used to observe cancellation requests for the associated
    /// operation.
    /// </summary>
    /// <remarks>Use this token to monitor for cancellation and respond appropriately in long-running or
    /// asynchronous operations. If no cancellation is requested, the token's <see
    /// cref="CancellationToken.IsCancellationRequested"/> property will be <see langword="false"/>.</remarks>
    public CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Gets a read-only collection of key/value pairs associated with the current context.
    /// </summary>
    /// <remarks>The returned dictionary provides access to all items stored in the context, using string
    /// keys. Modifying the returned collection is not supported.</remarks>
    public IReadOnlyDictionary<string, object> Items => _items.AsReadOnly();

    /// <summary>
    /// Gets the elapsed time between the start and finish of the operation, or between the start and the current time
    /// if the operation is not finished.
    /// </summary>
    /// <remarks>If the operation has not finished, the duration is calculated up to the current UTC time.
    /// This property is useful for tracking the progress or total time taken by the operation.</remarks>
    [JsonIgnore]
    public TimeSpan Duration => FinishedAt != default
        ? FinishedAt - StartedAt
        : DateTime.UtcNow - StartedAt;

    /// <summary>
    /// Initializes a new instance of the RequestContext class with the specified request, user identifier, and
    /// cancellation token.
    /// </summary>
    /// <param name="request">The request object associated with this context. Cannot be null.</param>
    /// <param name="userId">The identifier of the user making the request.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the request operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
    public RequestContext(TRequest request, string userId, CancellationToken cancellationToken = default)
    {
        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
        CancellationToken = cancellationToken;
    }

    /// <see cref="IIMMutableRequestContext{TRequest, TResponse}.AddItem(string, object)"/>
    public void AddItem(string key, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        _items[key] = value;
    }

    /// <see cref="IIMMutableRequestContext{TRequest, TResponse}.TryGetItem{T}(string, out T)"/>
    public bool TryGetItem<T>(string key, out T value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(_items);

        if (_items.TryGetValue(key, out var obj) && obj is T casted)
        {
            value = casted;
            return true;
        }
        value = default;
        return false;
    }

    /// <see cref="IIMMutableRequestContext{TRequest, TResponse}.RemoveItem(string)"/>
    public bool RemoveItem(string key) => _items.Remove(key);
}
