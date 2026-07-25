using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Mediarq.AzureServiceBus.Tests.Fixtures;
using Moq;

namespace Mediarq.AzureServiceBus.Tests;

public class AzureServiceBusNotificationForwarderTests
{
    [Fact]
    public async Task Handle_Sends_The_Message_On_The_Notifications_Topic()
    {
        var sender = new Mock<ServiceBusSender>();
        sender
            .Setup(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var client = new Mock<ServiceBusClient>();
        client.Setup(c => c.CreateSender("orders")).Returns(sender.Object);
        var forwarder = new AzureServiceBusNotificationForwarder<OrderPlaced>(client.Object);
        var notification = new OrderPlaced(Guid.NewGuid(), "Alice");

        await forwarder.Handle(notification);

        sender.Verify(s => s.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        client.Verify(c => c.CreateSender("orders"), Times.Once);
    }

    [Fact]
    public async Task Handle_Throws_When_Notification_Is_Null()
    {
        var client = new Mock<ServiceBusClient>();
        var forwarder = new AzureServiceBusNotificationForwarder<OrderPlaced>(client.Object);

        var act = async () => await forwarder.Handle(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Throws_When_Client_Is_Null()
    {
        var act = () => new AzureServiceBusNotificationForwarder<OrderPlaced>(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
