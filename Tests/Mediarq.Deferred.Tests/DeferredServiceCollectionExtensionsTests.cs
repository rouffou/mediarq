using System.Threading.Channels;
using FluentAssertions;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Mediarq.Deferred.Tests;

public class DeferredServiceCollectionExtensionsTests
{
    private static IServiceCollection NewServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new Mock<ISender>().Object);
        services.AddSingleton(new Mock<IPublisher>().Object);
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger<DeferredDispatchHostedService>>(
            NullLogger<DeferredDispatchHostedService>.Instance);
        return services;
    }

    [Fact]
    public void AddMediarqDeferredDispatch_Registers_IDeferredDispatcher()
    {
        var services = NewServices();

        services.AddMediarqDeferredDispatch();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IDeferredDispatcher>().Should().NotBeNull();
    }

    [Fact]
    public void AddMediarqDeferredDispatch_Registers_The_Hosted_Service()
    {
        var services = NewServices();

        services.AddMediarqDeferredDispatch();
        using var provider = services.BuildServiceProvider();

        provider.GetServices<IHostedService>().Should().ContainSingle(s => s is DeferredDispatchHostedService);
    }

    [Fact]
    public void AddMediarqDeferredDispatch_Returns_The_Same_ServiceCollection_For_Chaining()
    {
        var services = NewServices();

        var result = services.AddMediarqDeferredDispatch();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddMediarqDeferredDispatch_Throws_When_Services_Is_Null()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqDeferredDispatch();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqDeferredDispatch_Honors_A_Bounded_Capacity()
    {
        var services = NewServices();

        services.AddMediarqDeferredDispatch(o => o.Capacity = 2);
        using var provider = services.BuildServiceProvider();

        var channel = provider.GetRequiredService<Channel<Func<IServiceProvider, CancellationToken, Task>>>();

        channel.Writer.TryWrite((_, _) => Task.CompletedTask).Should().BeTrue();
        channel.Writer.TryWrite((_, _) => Task.CompletedTask).Should().BeTrue();
        channel.Writer.TryWrite((_, _) => Task.CompletedTask).Should().BeFalse("the channel is bounded to 2 items and both slots are already full");
    }
}
