namespace Mediarq.Samples.WebApi.Domain;

public enum OrderStatus
{
    Pending,
    Confirmed,
    Cancelled,
}

/// <summary>The order aggregate persisted through EF Core (the unit of work).</summary>
public sealed class Order
{
    public Guid Id { get; set; }
    public string Customer { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItem> Items { get; set; } = [];
    public string? Note { get; set; }
}

public sealed class OrderItem
{
    public Guid Id { get; set; }
    public string Product { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

/// <summary>Read model returned by queries (keeps the persistence entity out of the API surface).</summary>
public sealed record OrderDto(Guid Id, string Customer, decimal Total, string Status, IReadOnlyList<OrderLineDto> Items)
{
    public static OrderDto From(Order order) => new(
        order.Id,
        order.Customer,
        order.Total,
        order.Status.ToString(),
        order.Items.Select(i => new OrderLineDto(i.Product, i.Quantity, i.UnitPrice)).ToArray());
}

public sealed record OrderLineDto(string Product, int Quantity, decimal UnitPrice);
