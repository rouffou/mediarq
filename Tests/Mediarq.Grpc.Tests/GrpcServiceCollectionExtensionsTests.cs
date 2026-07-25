using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Grpc.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Grpc.Tests;

public class GrpcServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMediarqGrpcPublisher_Registers_The_Forwarder()
    {
        var services = new ServiceCollection();

        services.AddMediarqGrpcPublisher<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<INotificationHandler<OrderPlaced>>()
            .Should().BeOfType<GrpcNotificationForwarder<OrderPlaced>>();
    }

    [Fact]
    public void AddMediarqGrpcPublisher_Registers_The_Channel_Cache_As_A_Singleton()
    {
        var services = new ServiceCollection();

        services.AddMediarqGrpcPublisher<OrderPlaced>();
        using var provider = services.BuildServiceProvider();

        var first = provider.GetRequiredService<GrpcChannelCache>();
        var second = provider.GetRequiredService<GrpcChannelCache>();

        second.Should().BeSameAs(first);
    }

    [Fact]
    public void AddMediarqGrpcPublisher_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqGrpcPublisher<OrderPlaced>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqGrpcSubscriptions_Registers_The_Registry()
    {
        var services = new ServiceCollection();

        services.AddMediarqGrpcSubscriptions();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<GrpcNotificationSubscriptionRegistry>().Should().NotBeNull();
    }

    [Fact]
    public void AddMediarqGrpcSubscriptions_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqGrpcSubscriptions();

        act.Should().Throw<ArgumentNullException>();
    }
}
