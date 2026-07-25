using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;
using Mediarq.Grpc.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.Grpc.Tests;

// GrpcChannelCache always dials a real address via GrpcChannel.ForAddress (no injectable HttpMessageHandler
// hook, unlike raw HttpClient-based packages), so unlike the TestServer-based round trip in
// GrpcEndpointRouteBuilderExtensionsTests, exercising the *forwarder* end to end needs a real bound
// socket — this is the one test in the suite that does so, over plain HTTP/2 (h2c) on a fixed loopback
// port matching LiveOrderPlaced.ServiceAddress.
public class GrpcNotificationForwarderEndToEndTests
{
    [Fact]
    public async Task Forwarder_Delivers_Over_A_Real_Socket_To_A_Live_Kestrel_Server()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.ConfigureKestrel(options =>
            options.ListenLocalhost(57123, listenOptions => listenOptions.Protocols = HttpProtocols.Http2));
        var publisher = new Mock<IPublisher>();
        builder.Services.AddSingleton(publisher.Object);
        builder.Services.AddMediarqGrpcSubscriptions();

        await using var app = builder.Build();
        app.MapMediarqGrpcNotificationService();
        app.MapMediarqGrpcSubscription<LiveOrderPlaced>();
        await app.StartAsync();

        var services = new ServiceCollection();
        services.AddMediarqGrpcPublisher<LiveOrderPlaced>();
        await using var provider = services.BuildServiceProvider();
        var forwarder = provider.GetRequiredService<INotificationHandler<LiveOrderPlaced>>();

        var orderId = Guid.NewGuid();
        await forwarder.Handle(new LiveOrderPlaced(orderId, "Alice"));

        publisher.Verify(
            p => p.Publish(It.Is<LiveOrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
