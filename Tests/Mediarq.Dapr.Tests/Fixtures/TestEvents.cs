namespace Mediarq.Dapr.Tests.Fixtures;

public sealed record OrderPlaced(Guid OrderId, string Customer) : IDaprPubSubEvent
{
    public static string PubsubName => "orders-pubsub";
    public static string Topic => "order-placed";
}
