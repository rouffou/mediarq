using System.Collections.Concurrent;
using Grpc.Net.Client;

namespace Mediarq.Grpc;

/// <summary>
/// Caches one long-lived <see cref="GrpcChannel"/> per remote service address, so
/// <see cref="GrpcNotificationForwarder{TNotification}"/> never opens a new HTTP/2 connection per publish.
/// Registered as a singleton; disposed by the container at shutdown, which disposes every cached channel.
/// </summary>
/// <remarks>
/// Unlike the Dapr/RabbitMQ/Azure Service Bus packages' "bring your own client" convention, a
/// <see cref="GrpcChannel"/> is not an external SDK object carrying credentials the app must own — it is
/// just a pooled HTTP/2 connection to an address already known at compile time via
/// <see cref="IGrpcNotificationEvent.ServiceAddress"/>, so this package safely owns its lifecycle.
/// </remarks>
public sealed class GrpcChannelCache : IDisposable
{
    private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new();

    // Public (rather than internal): Microsoft.Extensions.DependencyInjection's default container only
    // considers public constructors when activating a type, so GrpcNotificationForwarder<TNotification>
    // (a public class DI must construct) cannot take an internal-typed constructor parameter.
    internal GrpcChannel GetOrCreate(string address)
        => _channels.GetOrAdd(address, static a => GrpcChannel.ForAddress(a));

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var channel in _channels.Values)
        {
            channel.Dispose();
        }
    }
}
