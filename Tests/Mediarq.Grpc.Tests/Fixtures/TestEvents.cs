namespace Mediarq.Grpc.Tests.Fixtures;

public sealed record OrderPlaced(Guid OrderId, string Customer) : IGrpcNotificationEvent
{
    public static string ServiceAddress => "https://order-service:5001";
}
