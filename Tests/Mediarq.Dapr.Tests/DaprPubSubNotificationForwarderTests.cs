using Dapr.Client;
using FluentAssertions;
using Mediarq.Dapr.Tests.Fixtures;
using Moq;

namespace Mediarq.Dapr.Tests;

public class DaprPubSubNotificationForwarderTests
{
    [Fact]
    public async Task Handle_Publishes_The_Notification_On_Its_Pubsub_Component_And_Topic()
    {
        var daprClient = new Mock<DaprClient>();
        daprClient
            .Setup(c => c.PublishEventAsync("orders-pubsub", "order-placed", It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var forwarder = new DaprPubSubNotificationForwarder<OrderPlaced>(daprClient.Object);
        var notification = new OrderPlaced(Guid.NewGuid(), "Alice");

        await forwarder.Handle(notification);

        daprClient.Verify(
            c => c.PublishEventAsync("orders-pubsub", "order-placed", notification, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_When_Notification_Is_Null()
    {
        var daprClient = new Mock<DaprClient>();
        var forwarder = new DaprPubSubNotificationForwarder<OrderPlaced>(daprClient.Object);

        var act = async () => await forwarder.Handle(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Throws_When_DaprClient_Is_Null()
    {
        var act = () => new DaprPubSubNotificationForwarder<OrderPlaced>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
