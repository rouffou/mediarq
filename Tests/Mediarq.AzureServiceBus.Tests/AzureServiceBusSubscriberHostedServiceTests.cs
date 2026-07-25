using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Mediarq.AzureServiceBus.Tests.Fixtures;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Mediarq.AzureServiceBus.Tests;

public class AzureServiceBusSubscriberHostedServiceTests
{
    private static (AzureServiceBusSubscriberHostedService<OrderPlaced> Service, Mock<ServiceBusProcessor> Processor, Mock<IPublisher> Publisher, Task Ready)
        CreateStartedService()
    {
        var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var processor = new Mock<ServiceBusProcessor>();
        processor
            .Setup(p => p.StartProcessingAsync(It.IsAny<CancellationToken>()))
            .Callback(() => ready.TrySetResult())
            .Returns(Task.CompletedTask);

        var client = new Mock<ServiceBusClient>();
        client.Setup(c => c.CreateProcessor("orders", "order-placed-subscribers")).Returns(processor.Object);

        var publisher = new Mock<IPublisher>();
        var services = new ServiceCollection();
        services.AddSingleton(publisher.Object);
        var provider = services.BuildServiceProvider();

        var service = new AzureServiceBusSubscriberHostedService<OrderPlaced>(
            client.Object,
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AzureServiceBusSubscriberHostedService<OrderPlaced>>.Instance);

        return (service, processor, publisher, ready.Task);
    }

    [Fact]
    public async Task ExecuteAsync_Creates_The_Processor_For_The_Notifications_Topic_And_Subscription_Then_Starts()
    {
        var (service, processor, _, ready) = CreateStartedService();

        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            processor.Verify(p => p.StartProcessingAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task OnProcessMessageAsync_Republishes_Through_IPublisher_And_Completes()
    {
        var (service, _, publisher, ready) = CreateStartedService();
        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            var orderId = Guid.NewGuid();
            var body = JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(orderId, "Alice"));
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(body: BinaryData.FromBytes(body));
            var receiver = new Mock<ServiceBusReceiver>();
            receiver.Setup(r => r.CompleteMessageAsync(message, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var args = new ProcessMessageEventArgs(message, receiver.Object, "identifier", CancellationToken.None);

            await service.OnProcessMessageAsync(args);

            publisher.Verify(
                p => p.Publish(It.Is<OrderPlaced>(n => n.OrderId == orderId && n.Customer == "Alice"), It.IsAny<CancellationToken>()),
                Times.Once);
            receiver.Verify(r => r.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
            receiver.Verify(
                r => r.DeadLetterMessageAsync(It.IsAny<ServiceBusReceivedMessage>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task OnProcessMessageAsync_DeadLetters_On_Malformed_Body()
    {
        var (service, _, publisher, ready) = CreateStartedService();
        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            var body = Encoding.UTF8.GetBytes("not json");
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(body: BinaryData.FromBytes(body));
            var receiver = new Mock<ServiceBusReceiver>();
            receiver
                .Setup(r => r.DeadLetterMessageAsync(message, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var args = new ProcessMessageEventArgs(message, receiver.Object, "identifier", CancellationToken.None);

            await service.OnProcessMessageAsync(args);

            publisher.Verify(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>()), Times.Never);
            receiver.Verify(r => r.DeadLetterMessageAsync(message, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()), Times.Once);
            receiver.Verify(r => r.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task OnProcessMessageAsync_DeadLetters_When_The_Publisher_Throws()
    {
        var (service, _, publisher, ready) = CreateStartedService();
        publisher.Setup(p => p.Publish(It.IsAny<OrderPlaced>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("boom"));
        await service.StartAsync(CancellationToken.None);
        try
        {
            await ready;
            var body = JsonSerializer.SerializeToUtf8Bytes(new OrderPlaced(Guid.NewGuid(), "Bob"));
            var message = ServiceBusModelFactory.ServiceBusReceivedMessage(body: BinaryData.FromBytes(body));
            var receiver = new Mock<ServiceBusReceiver>();
            receiver
                .Setup(r => r.DeadLetterMessageAsync(message, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var args = new ProcessMessageEventArgs(message, receiver.Object, "identifier", CancellationToken.None);

            await service.OnProcessMessageAsync(args);

            receiver.Verify(r => r.DeadLetterMessageAsync(message, It.IsAny<IDictionary<string, object>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public void Constructor_Throws_When_Client_Is_Null()
    {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();

        var act = () => new AzureServiceBusSubscriberHostedService<OrderPlaced>(
            null!, provider.GetRequiredService<IServiceScopeFactory>(), NullLogger<AzureServiceBusSubscriberHostedService<OrderPlaced>>.Instance);

        act.Should().Throw<ArgumentNullException>();
    }
}
