using System.Text;
using System.Text.Json;
using FluentAssertions;
using Mediarq.Core.Mediators;
using Mediarq.RabbitMQ.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mediarq.RabbitMQ.Tests;

public class RabbitMqSubscriberHostedServiceTests
{
    private static (RabbitMqSubscriberHostedService<OrderPlaced> Service, Mock<IChannel> Channel, Mock<IPublisher> Publisher, Task Ready)
        CreateStartedService()
    {
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var channel = new Mock<IChannel>();
        channel
            .Setup(c => c.BasicConsumeAsync(
                It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<IDictionary<string, object?>?>(), It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()))
            .Callback(() => ready.TrySetResult())
            .ReturnsAsync("consumer-tag");

        var connection = new Mock<IConnection>();
        connection
            .Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(channel.Object);

        var publisher = new Mock<IPublisher>();
        var services = new ServiceCollection();
        services.AddSingleton(publisher.Object);
        var provider = services.BuildServiceProvider();

        var service = new RabbitMqSubscriberHostedService<OrderPlaced>(
            connection.Object,
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<RabbitMqSubscriberHostedService<OrderPlaced>>.Instance);

        return (service, channel, publisher, ready.Task);
    }

    [Fact]
    public async Task ExecuteAsync_Declares_The_Exchange_Queue_And_Binding_Then_Consumes()
    {
        var (service, channel, _, ready) = CreateStartedService();

        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            channel.Verify(c => c.ExchangeDeclareAsync(
                "orders", ExchangeType.Direct, true, false,
                It.IsAny<IDictionary<string, object?>?>(), false, false, It.IsAny<CancellationToken>()), Times.Once);
            channel.Verify(c => c.QueueDeclareAsync(
                "orders.order-placed", true, false, false,
                It.IsAny<IDictionary<string, object?>?>(), false, false, It.IsAny<CancellationToken>()), Times.Once);
            channel.Verify(c => c.QueueBindAsync(
                "orders.order-placed", "orders", "order-placed",
                It.IsAny<IDictionary<string, object?>?>(), false, It.IsAny<CancellationToken>()), Times.Once);
            channel.Verify(c => c.BasicConsumeAsync(
                "orders.order-placed", false, It.IsAny<string>(), false, false,
                It.IsAny<IDictionary<string, object?>?>(), It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task OnReceivedAsync_Republishes_Through_IPublisher_And_Acks()
    {
        var (service, channel, publisher, ready) = CreateStartedService();
        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            var orderId = Guid.NewGuid();
            var body = JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(orderId, "Alice"));
            var args = new BasicDeliverEventArgs("consumer-tag", 7UL, false, "orders", "order-placed", new BasicProperties(), body);

            await service.OnReceivedAsync(this, args);

            publisher.Verify(
                p => p.Publish(It.Is<OrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
                Times.Once);
            channel.Verify(c => c.BasicAckAsync(7UL, false, It.IsAny<CancellationToken>()), Times.Once);
            channel.Verify(c => c.BasicNackAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task OnReceivedAsync_Nacks_Without_Requeue_On_Malformed_Body()
    {
        var (service, channel, publisher, ready) = CreateStartedService();
        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            var body = Encoding.UTF8.GetBytes("not json");
            var args = new BasicDeliverEventArgs("consumer-tag", 3UL, false, "orders", "order-placed", new BasicProperties(), body);

            await service.OnReceivedAsync(this, args);

            publisher.Verify(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>()), Times.Never);
            channel.Verify(c => c.BasicNackAsync(3UL, false, false, It.IsAny<CancellationToken>()), Times.Once);
            channel.Verify(c => c.BasicAckAsync(It.IsAny<ulong>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task OnReceivedAsync_Nacks_Without_Requeue_When_The_Publisher_Throws()
    {
        var (service, channel, publisher, ready) = CreateStartedService();
        publisher.Setup(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("boom"));
        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            var body = JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(Guid.NewGuid(), "Bob"));
            var args = new BasicDeliverEventArgs("consumer-tag", 9UL, false, "orders", "order-placed", new BasicProperties(), body);

            await service.OnReceivedAsync(this, args);

            channel.Verify(c => c.BasicNackAsync(9UL, false, false, It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public void Constructor_Throws_When_Connection_Is_Null()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var act = () => new RabbitMqSubscriberHostedService<OrderPlaced>(
            null!, provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<RabbitMqSubscriberHostedService<OrderPlaced>>.Instance);

        act.Should().Throw<ArgumentNullException>();
    }
}
