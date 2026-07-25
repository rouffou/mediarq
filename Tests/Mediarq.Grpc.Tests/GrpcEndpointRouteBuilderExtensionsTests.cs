using FluentAssertions;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Mediarq.Core.Mediators;
using Mediarq.Grpc.Contracts;
using Mediarq.Grpc.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.Grpc.Tests;

public class GrpcEndpointRouteBuilderExtensionsTests
{
    private static async Task<(WebApplication App, Mock<IPublisher> Publisher)> CreateAppAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        var publisher = new Mock<IPublisher>();
        builder.Services.AddSingleton(publisher.Object);
        builder.Services.AddMediarqGrpcSubscriptions();

        var app = builder.Build();
        app.MapMediarqGrpcNotificationService();
        app.MapMediarqGrpcSubscription<OrderPlaced>();
        await app.StartAsync();
        return (app, publisher);
    }

    [Fact]
    public void MapMediarqGrpcNotificationService_Throws_When_App_Is_Null()
    {
        Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app = null!;

        var act = () => app.MapMediarqGrpcNotificationService();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapMediarqGrpcSubscription_Throws_When_App_Is_Null()
    {
        Microsoft.AspNetCore.Routing.IEndpointRouteBuilder app = null!;

        var act = () => app.MapMediarqGrpcSubscription<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Publish_Over_The_Real_gRPC_Endpoint_Republishes_Through_IPublisher()
    {
        var (app, publisher) = await CreateAppAsync();
        await using var _1 = app;

        using var channel = GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions { HttpHandler = app.GetTestServer().CreateHandler() });
        var client = new NotificationService.NotificationServiceClient(channel);
        var orderId = Guid.NewGuid();

        var envelope = new NotificationEnvelope
        {
            TypeName = typeof(OrderPlaced).FullName!,
            Payload = ByteString.CopyFrom(
                System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(orderId, "Alice"))),
        };

        var ack = await client.PublishAsync(envelope);

        ack.Accepted.Should().BeTrue();
        publisher.Verify(
            p => p.Publish(It.Is<OrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Publish_Over_The_Real_gRPC_Endpoint_Fails_NotFound_For_An_Unsubscribed_Type()
    {
        var (app, _) = await CreateAppAsync();
        await using var _1 = app;

        using var channel = GrpcChannel.ForAddress(
            "http://localhost",
            new GrpcChannelOptions { HttpHandler = app.GetTestServer().CreateHandler() });
        var client = new NotificationService.NotificationServiceClient(channel);

        var envelope = new NotificationEnvelope { TypeName = "Unregistered.Type", Payload = ByteString.Empty };

        var act = async () => await client.PublishAsync(envelope);

        var exception = await act.Should().ThrowAsync<RpcException>();
        exception.Which.StatusCode.Should().Be(StatusCode.NotFound);
    }
}
