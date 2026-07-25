namespace Mediarq.AzureServiceBus.Tests.Fixtures;

public sealed record OrderPlaced(Guid OrderId, string Customer) : IAzureServiceBusEvent
{
    public static string TopicName => "orders";
    public static string SubscriptionName => "order-placed-subscribers";
}
