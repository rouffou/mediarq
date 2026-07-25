using FluentAssertions;
using Mediarq.RabbitMQ.Tests.Fixtures;
using Moq;
using RabbitMQ.Client;

namespace Mediarq.RabbitMQ.Tests;

public class RabbitMqNotificationForwarderTests
{
    [Fact]
    public async Task Handle_Publishes_On_The_Notifications_Exchange_And_RoutingKey()
    {
        var channel = new Mock<IChannel>();
        var connection = new Mock<IConnection>();
        connection
            .Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel.Object);
        var forwarder = new RabbitMqNotificationForwarder<OrderPlaced>(connection.Object);
        var notification = new OrderPlaced(Guid.NewGuid(), "Alice");

        await forwarder.Handle(notification);

        channel.Verify(
            c => c.BasicPublishAsync("orders", "order-placed", false, It.IsAny<BasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_When_Notification_Is_Null()
    {
        var connection = new Mock<IConnection>();
        var forwarder = new RabbitMqNotificationForwarder<OrderPlaced>(connection.Object);

        var act = async () => await forwarder.Handle(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Throws_When_Connection_Is_Null()
    {
        var act = () => new RabbitMqNotificationForwarder<OrderPlaced>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
