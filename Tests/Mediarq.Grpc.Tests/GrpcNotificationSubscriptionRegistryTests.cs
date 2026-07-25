using System.Text.Json;
using FluentAssertions;
using Google.Protobuf;
using Mediarq.Core.Mediators;
using Mediarq.Grpc.Tests.Fixtures;
using Moq;

namespace Mediarq.Grpc.Tests;

public class GrpcNotificationSubscriptionRegistryTests
{
    [Fact]
    public void TryGetHandler_Returns_False_For_An_Unregistered_Type()
    {
        var registry = new GrpcNotificationSubscriptionRegistry();

        var found = registry.TryGetHandler("Unknown.Type", out _);

        found.Should().BeFalse();
    }

    [Fact]
    public async Task TryGetHandler_Returns_A_Delegate_That_Deserializes_And_Publishes()
    {
        var registry = new GrpcNotificationSubscriptionRegistry();
        registry.Add<OrderPlaced>();

        var found = registry.TryGetHandler(typeof(OrderPlaced).FullName!, out var handler);
        found.Should().BeTrue();

        var orderId = Guid.NewGuid();
        var payload = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(orderId, "Alice")));
        var publisher = new Mock<IPublisher>();

        await handler(payload, publisher.Object, CancellationToken.None);

        publisher.Verify(
            p => p.Publish(It.Is<OrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task TryGetHandler_Delegate_Throws_When_The_Payload_Deserializes_To_Null()
    {
        var registry = new GrpcNotificationSubscriptionRegistry();
        registry.Add<OrderPlaced>();
        registry.TryGetHandler(typeof(OrderPlaced).FullName!, out var handler);

        var nullPayload = ByteString.CopyFromUtf8("null");
        var publisher = new Mock<IPublisher>();

        var act = async () => await handler(nullPayload, publisher.Object, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
