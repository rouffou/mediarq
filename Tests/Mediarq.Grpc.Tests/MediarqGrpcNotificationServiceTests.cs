using System.Text.Json;
using FluentAssertions;
using Google.Protobuf;
using Grpc.Core;
using Mediarq.Core.Mediators;
using Mediarq.Grpc.Contracts;
using Mediarq.Grpc.Tests.Fixtures;
using Moq;

namespace Mediarq.Grpc.Tests;

public class MediarqGrpcNotificationServiceTests
{
    [Fact]
    public async Task Publish_Republishes_A_Known_Notification_Type_And_Returns_Accepted()
    {
        var registry = new GrpcNotificationSubscriptionRegistry();
        registry.Add<OrderPlaced>();
        var publisher = new Mock<IPublisher>();
        var service = new MediarqGrpcNotificationService(registry, publisher.Object);

        var orderId = Guid.NewGuid();
        var envelope = new NotificationEnvelope
        {
            TypeName = typeof(OrderPlaced).FullName!,
            Payload = ByteString.CopyFrom(JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(orderId, "Alice"))),
        };

        var ack = await service.Publish(envelope, TestServerCallContext.Create());

        ack.Accepted.Should().BeTrue();
        publisher.Verify(
            p => p.Publish(It.Is<OrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Publish_Throws_NotFound_For_An_Unregistered_Notification_Type()
    {
        var registry = new GrpcNotificationSubscriptionRegistry();
        var publisher = new Mock<IPublisher>();
        var service = new MediarqGrpcNotificationService(registry, publisher.Object);

        var envelope = new NotificationEnvelope { TypeName = "Some.Unknown.Type", Payload = ByteString.Empty };

        var act = async () => await service.Publish(envelope, TestServerCallContext.Create());

        var exception = await act.Should().ThrowAsync<RpcException>();
        exception.Which.StatusCode.Should().Be(StatusCode.NotFound);
    }

    // Minimal ServerCallContext for unit-testing a service method directly, without a real gRPC call.
    private sealed class TestServerCallContext : ServerCallContext
    {
        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;
        protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options) => null!;
        protected override string MethodCore => "Publish";
        protected override string HostCore => "test";
        protected override string PeerCore => "test";
        protected override DateTime DeadlineCore => DateTime.MaxValue;
        protected override Metadata RequestHeadersCore { get; } = [];
        protected override CancellationToken CancellationTokenCore { get; } = CancellationToken.None;
        protected override Metadata ResponseTrailersCore { get; } = [];
        protected override Status StatusCore { get; set; }
        protected override WriteOptions? WriteOptionsCore { get; set; }
        protected override AuthContext AuthContextCore { get; } = new("test", new Dictionary<string, List<AuthProperty>>());

        internal static TestServerCallContext Create() => new();
    }
}
