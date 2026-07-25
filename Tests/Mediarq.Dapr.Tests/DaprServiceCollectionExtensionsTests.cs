using Dapr.Client;
using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Dapr.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.Dapr.Tests;

public class DaprServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMediarqDaprPubSub_Generic_Registers_The_Forwarder()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<DaprClient>().Object);

        services.AddMediarqDaprPubSub<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<INotificationHandler<OrderPlaced>>()
            .Should().BeOfType<DaprPubSubNotificationForwarder<OrderPlaced>>();
    }

    [Fact]
    public void AddMediarqDaprPubSub_Assembly_Scan_Registers_Every_IDaprPubSubEvent()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<DaprClient>().Object);

        services.AddMediarqDaprPubSub(typeof(OrderPlaced).Assembly);
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<INotificationHandler<OrderPlaced>>()
            .Should().BeOfType<DaprPubSubNotificationForwarder<OrderPlaced>>();
    }

    [Fact]
    public void AddMediarqDaprPubSub_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqDaprPubSub<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqDaprPubSubSubscriptions_Registers_The_Registry()
    {
        var services = new ServiceCollection();

        services.AddMediarqDaprPubSubSubscriptions();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<DaprPubSubSubscriptionRegistry>().Should().NotBeNull();
    }

    [Fact]
    public void AddMediarqDaprPubSubSubscriptions_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqDaprPubSubSubscriptions();

        act.Should().Throw<ArgumentNullException>();
    }
}
