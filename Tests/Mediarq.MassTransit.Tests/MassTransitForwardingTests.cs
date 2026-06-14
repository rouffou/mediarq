using FluentAssertions;
using MassTransit;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.MassTransit.Tests;

public class MassTransitForwardingTests
{
    public record OrderShipped(int OrderId) : IIntegrationEvent;

    public record InternalOnly(int Id) : INotification;

    [Fact]
    public async Task Forwarder_Publishes_Notification_To_Endpoint()
    {
        var endpoint = new Mock<IPublishEndpoint>();
        endpoint
            .Setup(e => e.Publish(It.IsAny<OrderShipped>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var forwarder = new MassTransitNotificationForwarder<OrderShipped>(endpoint.Object);
        var notification = new OrderShipped(7);

        await forwarder.Handle(notification);

        endpoint.Verify(e => e.Publish(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Forwarder_Throws_When_Endpoint_Is_Null()
    {
        var act = () => new MassTransitNotificationForwarder<OrderShipped>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("publishEndpoint");
    }

    [Fact]
    public void AddForwarding_Generic_Registers_Forwarder_As_NotificationHandler()
    {
        var services = new ServiceCollection();

        services.AddMediarqMassTransitForwarding<OrderShipped>();

        var descriptor = services.Single(d => d.ServiceType == typeof(INotificationHandler<OrderShipped>));
        descriptor.ImplementationType.Should().Be(typeof(MassTransitNotificationForwarder<OrderShipped>));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddForwarding_Scan_Registers_Only_IntegrationEvents()
    {
        var services = new ServiceCollection();

        services.AddMediarqMassTransitForwarding(typeof(MassTransitForwardingTests).Assembly);

        // OrderShipped is an IIntegrationEvent -> forwarded.
        services.Should().ContainSingle(d => d.ServiceType == typeof(INotificationHandler<OrderShipped>)
            && d.ImplementationType == typeof(MassTransitNotificationForwarder<OrderShipped>));

        // InternalOnly is a plain INotification -> NOT forwarded.
        services.Should().NotContain(d => d.ServiceType == typeof(INotificationHandler<InternalOnly>));
    }
}
