using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Mediarq.AzureServiceBus.Tests.Fixtures;
using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Mediarq.AzureServiceBus.Tests;

public class AzureServiceBusServiceCollectionExtensionsTests
{
    private static IServiceCollection NewServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<ServiceBusClient>().Object);
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger<AzureServiceBusSubscriberHostedService<OrderPlaced>>>(
            NullLogger<AzureServiceBusSubscriberHostedService<OrderPlaced>>.Instance);
        return services;
    }

    [Fact]
    public void AddMediarqAzureServiceBusPublisher_Registers_The_Forwarder()
    {
        var services = NewServices();

        services.AddMediarqAzureServiceBusPublisher<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<INotificationHandler<OrderPlaced>>()
            .Should().BeOfType<AzureServiceBusNotificationForwarder<OrderPlaced>>();
    }

    [Fact]
    public void AddMediarqAzureServiceBusPublisher_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqAzureServiceBusPublisher<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqAzureServiceBusSubscriber_Registers_The_Hosted_Service()
    {
        var services = NewServices();

        services.AddMediarqAzureServiceBusSubscriber<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        provider.GetServices<IHostedService>().Should().ContainSingle(s => s is AzureServiceBusSubscriberHostedService<OrderPlaced>);
    }

    [Fact]
    public void AddMediarqAzureServiceBusSubscriber_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqAzureServiceBusSubscriber<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }
}
