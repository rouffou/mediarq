using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Mediarq.Core.Mediators;
using Mediarq.Dapr.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.Dapr.Tests;

public class DaprPubSubEndpointRouteBuilderExtensionsTests
{
    private static async Task<(WebApplication App, Mock<IPublisher> Publisher)> CreateAppAsync(string? route = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        var publisher = new Mock<IPublisher>();
        builder.Services.AddSingleton(publisher.Object);
        builder.Services.AddMediarqDaprPubSubSubscriptions();

        var app = builder.Build();
        app.MapDaprPubSubSubscription<OrderPlaced>(route);
        app.MapDaprPubSubSubscribeEndpoint();
        await app.StartAsync();
        return (app, publisher);
    }

    [Fact]
    public async Task Subscribe_Discovery_Endpoint_Lists_The_Registered_Subscription()
    {
        var (app, _) = await CreateAppAsync();
        await using var _1 = app;
        var client = app.GetTestClient();

        var subscriptions = await client.GetFromJsonAsync<List<DaprSubscription>>("/dapr/subscribe");

        subscriptions.Should().ContainSingle(s =>
            s.PubsubName == "orders-pubsub" && s.Topic == "order-placed" && s.Route == "/dapr/pubsub/OrderPlaced");
    }

    [Fact]
    public async Task Subscribe_Discovery_Endpoint_Honors_A_Custom_Route()
    {
        var (app, _) = await CreateAppAsync("/webhooks/order-placed");
        await using var _1 = app;
        var client = app.GetTestClient();

        var subscriptions = await client.GetFromJsonAsync<List<DaprSubscription>>("/dapr/subscribe");

        subscriptions.Should().ContainSingle(s => s.Route == "/webhooks/order-placed");
    }

    [Fact]
    public async Task Webhook_Republishes_The_CloudEvent_Data_Through_IPublisher()
    {
        var (app, publisher) = await CreateAppAsync();
        await using var _1 = app;
        var client = app.GetTestClient();
        var orderId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync("/dapr/pubsub/OrderPlaced", new
        {
            data = new { orderId, customer = "Alice" },
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        publisher.Verify(
            p => p.Publish(It.Is<OrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Webhook_Returns_400_When_Data_Is_Missing()
    {
        var (app, publisher) = await CreateAppAsync();
        await using var _1 = app;
        var client = app.GetTestClient();

        var response = await client.PostAsJsonAsync("/dapr/pubsub/OrderPlaced", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        publisher.Verify(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
