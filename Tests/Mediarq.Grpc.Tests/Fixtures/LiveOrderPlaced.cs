namespace Mediarq.Grpc.Tests.Fixtures;

// A separate fixture (rather than reusing OrderPlaced) because ServiceAddress must point at a real,
// locally-bound Kestrel endpoint for the one test that exercises the actual client-to-server socket path
// (GrpcNotificationForwarderEndToEndTests) — every other test only needs a well-formed address string.
public sealed record LiveOrderPlaced(Guid OrderId, string Customer) : IGrpcNotificationEvent
{
    public static string ServiceAddress => "http://127.0.0.1:57123";
}
