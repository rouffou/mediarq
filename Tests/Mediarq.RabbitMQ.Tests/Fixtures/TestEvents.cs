namespace Mediarq.RabbitMQ.Tests.Fixtures;

public sealed record OrderPlaced(Guid OrderId, string Customer) : IRabbitMqEvent
{
    public static string Exchange => "orders";
    public static string Queue => "orders.order-placed";
    public static string RoutingKey => "order-placed";
}
