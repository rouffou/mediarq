using Mediarq.Core.Common.Requests.Abstraction;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Mediarq.Core.Common.Contexts;

[DebuggerDisplay("RequestContext {RequestId} ({typeof(TRequest).Name})")]
public record RequestContext<TRequest, TResponse>
    : IMutableRequestContext<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly IDictionary<string, object> _items = new Dictionary<string, object>();

    public Guid RequestId { get; init; } = Guid.NewGuid();

    public Guid CorrelationId { get; init; } = Guid.NewGuid();

    public string UserId { get; init; }
    
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;

    public DateTime FinishedAt { get; set; }

    public TRequest Request { get; init; }

    public CancellationToken CancellationToken { get; init; }

    public IReadOnlyDictionary<string, object> Items => _items.AsReadOnly();

    [JsonIgnore]
    public TimeSpan Duration => FinishedAt != default
        ? FinishedAt - StartedAt
        : DateTime.UtcNow - StartedAt;

    public RequestContext(TRequest request, string userId, CancellationToken cancellationToken = default)
    {
        UserId = userId;
        Request = request ?? throw new ArgumentNullException(nameof(request));
        CancellationToken = cancellationToken;
    }

    public void AddItem(string key, object value) => _items[key] = value;

    public bool TryGetItem<T>(string key, out T value)
    {
        if (_items.TryGetValue(key, out var obj) && obj is T casted)
        {
            value = casted;
            return true;
        }
        value = default;
        return false;
    }

    public bool RemoveItem(string key) => _items.Remove(key);
}
