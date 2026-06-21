using System.Collections.Concurrent;

namespace Mediarq.Samples.Quickstart.Domain;

/// <summary>Lifecycle of an <see cref="Order"/>.</summary>
public enum OrderStatus
{
    Pending,
    Confirmed,
    Cancelled,
}

/// <summary>A tiny order aggregate used throughout the quickstart.</summary>
public sealed record Order
{
    public Guid Id { get; init; }
    public string Customer { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public OrderStatus Status { get; init; } = OrderStatus.Pending;
}

/// <summary>Abstraction over order persistence so handlers stay testable.</summary>
public interface IOrderStore
{
    void Add(Order order);
    Order? Find(Guid id);
    void Update(Order order);
    IReadOnlyCollection<Order> All();
}

/// <summary>
/// In-memory, thread-safe store registered as a singleton. Real apps would inject a repository or a
/// <c>DbContext</c> here instead — see the WebApi sample for the EF Core / unit-of-work story.
/// </summary>
public sealed class InMemoryOrderStore : IOrderStore
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public void Add(Order order) => _orders[order.Id] = order;

    public Order? Find(Guid id) => _orders.TryGetValue(id, out var order) ? order : null;

    public void Update(Order order) => _orders[order.Id] = order;

    public IReadOnlyCollection<Order> All() => _orders.Values.ToArray();
}
