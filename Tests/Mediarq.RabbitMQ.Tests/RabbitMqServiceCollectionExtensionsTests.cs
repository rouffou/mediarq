using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.RabbitMQ.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using RabbitMQ.Client;

namespace Mediarq.RabbitMQ.Tests;

public class RabbitMqServiceCollectionExtensionsTests
{
    private static IServiceCollection NewServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<IConnection>().Object);
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger<RabbitMqSubscriberHostedService<OrderPlaced>>>(
            NullLogger<RabbitMqSubscriberHostedService<OrderPlaced>>.Instance);
        return services;
    }

    [Fact]
    public void AddMediarqRabbitMqPublisher_Registers_The_Forwarder()
    {
        var services = NewServices();

        services.AddMediarqRabbitMqPublisher<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<INotificationHandler<OrderPlaced>>()
            .Should().BeOfType<RabbitMqNotificationForwarder<OrderPlaced>>();
    }

    [Fact]
    public void AddMediarqRabbitMqPublisher_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqRabbitMqPublisher<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqRabbitMqSubscriber_Registers_The_Hosted_Service()
    {
        var services = NewServices();

        services.AddMediarqRabbitMqSubscriber<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        provider.GetServices<IHostedService>().Should().ContainSingle(s => s is RabbitMqSubscriberHostedService<OrderPlaced>);
    }

    [Fact]
    public void AddMediarqRabbitMqSubscriber_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqRabbitMqSubscriber<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }
}
