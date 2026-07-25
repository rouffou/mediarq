using FluentAssertions;
using Mediarq.Core.Mediators;
using Mediarq.Deferred.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Mediarq.Deferred.Tests;

public class ChannelDeferredDispatcherTests
{
    private static ServiceProvider BuildProvider(
        Mock<ISender> sender,
        Mock<IPublisher> publisher,
        Action<DeferredDispatchOptions>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(sender.Object);
        services.AddSingleton(publisher.Object);
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger<DeferredDispatchHostedService>>(
            NullLogger<DeferredDispatchHostedService>.Instance);
        services.AddMediarqDeferredDispatch(configure);
        return services.BuildServiceProvider();
    }

    private static DeferredDispatchHostedService GetHostedService(ServiceProvider provider) =>
        provider.GetServices<IHostedService>().OfType<DeferredDispatchHostedService>().Single();

    [Fact]
    public async Task SendLaterAsync_Eventually_Dispatches_Through_ISender()
    {
        var sender = new Mock<ISender>();
        var publisher = new Mock<IPublisher>();
        var tcs = new TaskCompletionSource();
        sender.Setup(s => s.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .Callback(() => tcs.TrySetResult())
            .Returns(Task.CompletedTask);

        await using var provider = BuildProvider(sender, publisher);
        var hosted = GetHostedService(provider);
        await hosted.StartAsync(CancellationToken.None);
        try
        {
            var dispatcher = provider.GetRequiredService<IDeferredDispatcher>();
            await dispatcher.SendLaterAsync(new TestCommand("hello"));

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(5)));
            completed.Should().Be(tcs.Task);
            sender.Verify(s => s.Send(It.Is<TestCommand>(c => c.Payload == "hello"), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await hosted.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task PublishLaterAsync_Eventually_Dispatches_Through_IPublisher()
    {
        var sender = new Mock<ISender>();
        var publisher = new Mock<IPublisher>();
        var tcs = new TaskCompletionSource();
        publisher.Setup(p => p.Publish(It.IsAny<TestNotification>(), It.IsAny<CancellationToken>()))
            .Callback(() => tcs.TrySetResult())
            .Returns(Task.CompletedTask);

        await using var provider = BuildProvider(sender, publisher);
        var hosted = GetHostedService(provider);
        await hosted.StartAsync(CancellationToken.None);
        try
        {
            var dispatcher = provider.GetRequiredService<IDeferredDispatcher>();
            await dispatcher.PublishLaterAsync(new TestNotification("world"));

            var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(5)));
            completed.Should().Be(tcs.Task);
            publisher.Verify(p => p.Publish(It.Is<TestNotification>(n => n.Payload == "world"), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            await hosted.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task StopAsync_Drains_Already_Queued_Work_Before_Completing()
    {
        var sender = new Mock<ISender>();
        var publisher = new Mock<IPublisher>();
        var processed = 0;
        sender.Setup(s => s.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await Task.Delay(20);
                Interlocked.Increment(ref processed);
            });

        await using var provider = BuildProvider(sender, publisher);
        var hosted = GetHostedService(provider);
        await hosted.StartAsync(CancellationToken.None);

        var dispatcher = provider.GetRequiredService<IDeferredDispatcher>();
        for (var i = 0; i < 5; i++)
        {
            await dispatcher.SendLaterAsync(new TestCommand($"item-{i}"));
        }

        await hosted.StopAsync(CancellationToken.None);

        processed.Should().Be(5);
    }

    [Fact]
    public async Task An_Exception_In_One_Item_Does_Not_Stop_Later_Items_From_Running()
    {
        var sender = new Mock<ISender>();
        var publisher = new Mock<IPublisher>();
        var secondItemRan = new TaskCompletionSource();
        sender.SetupSequence(s => s.Send(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("boom"))
            .Returns(Task.CompletedTask);
        sender.Setup(s => s.Send(It.Is<TestCommand>(c => c.Payload == "second"), It.IsAny<CancellationToken>()))
            .Callback(() => secondItemRan.TrySetResult())
            .Returns(Task.CompletedTask);

        await using var provider = BuildProvider(sender, publisher);
        var hosted = GetHostedService(provider);
        await hosted.StartAsync(CancellationToken.None);
        try
        {
            var dispatcher = provider.GetRequiredService<IDeferredDispatcher>();
            await dispatcher.SendLaterAsync(new TestCommand("first"));
            await dispatcher.SendLaterAsync(new TestCommand("second"));

            var completed = await Task.WhenAny(secondItemRan.Task, Task.Delay(TimeSpan.FromSeconds(5)));
            completed.Should().Be(secondItemRan.Task);
        }
        finally
        {
            await hosted.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task SendLaterAsync_Throws_When_Command_Is_Null()
    {
        var sender = new Mock<ISender>();
        var publisher = new Mock<IPublisher>();
        await using var provider = BuildProvider(sender, publisher);
        var dispatcher = provider.GetRequiredService<IDeferredDispatcher>();

        var act = async () => await dispatcher.SendLaterAsync<TestCommand>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PublishLaterAsync_Throws_When_Notification_Is_Null()
    {
        var sender = new Mock<ISender>();
        var publisher = new Mock<IPublisher>();
        await using var provider = BuildProvider(sender, publisher);
        var dispatcher = provider.GetRequiredService<IDeferredDispatcher>();

        var act = async () => await dispatcher.PublishLaterAsync<TestNotification>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
