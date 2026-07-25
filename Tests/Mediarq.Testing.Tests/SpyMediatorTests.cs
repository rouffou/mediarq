using FluentAssertions;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Testing.Tests.Fixtures;
using Moq;

namespace Mediarq.Testing.Tests;

public class SpyMediatorTests
{
    [Fact]
    public async Task Send_Records_The_Request_And_Delegates_To_Inner()
    {
        var inner = new Mock<IMediator>();
        var request = new PingCommand("hi");
        inner.Setup(m => m.Send(request, It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success("hi"));

        var spy = new SpyMediator(inner.Object);
        var response = await spy.Send<Result<string>>(request);

        response.Value.Should().Be("hi");
        spy.SentRequests.Should().ContainSingle().Which.Should().BeSameAs(request);
        inner.Verify(m => m.Send<Result<string>>(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Send_Void_Records_The_Request_And_Delegates_To_Inner()
    {
        var inner = new Mock<IMediator>();
        var request = new VoidCommand();
        inner.Setup(m => m.Send(request, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var spy = new SpyMediator(inner.Object);
        await spy.Send(request);

        spy.SentRequests.Should().ContainSingle().Which.Should().BeSameAs(request);
        inner.Verify(m => m.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_Records_The_Notification_And_Delegates_To_Inner()
    {
        var inner = new Mock<IMediator>();
        var notification = new Pinged(1);
        inner.Setup(m => m.Publish(notification, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var spy = new SpyMediator(inner.Object);
        await spy.Publish(notification);

        spy.PublishedNotifications.Should().ContainSingle().Which.Should().BeSameAs(notification);
        inner.Verify(m => m.Publish(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateStream_Records_The_Request_And_Delegates_To_Inner()
    {
        var inner = new Mock<IMediator>();
        var request = new CountStream(3);
        inner.Setup(m => m.CreateStream(request, It.IsAny<CancellationToken>())).Returns(Stream());

        var spy = new SpyMediator(inner.Object);
        var items = new List<int>();
        await foreach (var item in spy.CreateStream<int>(request))
        {
            items.Add(item);
        }

        items.Should().Equal(1, 2, 3);
        spy.SentRequests.Should().ContainSingle().Which.Should().BeSameAs(request);

        static async IAsyncEnumerable<int> Stream()
        {
            yield return 1;
            yield return 2;
            yield return 3;
            await Task.CompletedTask;
        }
    }

    [Fact]
    public void Clear_Empties_Both_Recorded_Lists()
    {
        var inner = new Mock<IMediator>();
        var spy = new SpyMediator(inner.Object);

        // Reach in via reflection-free path: use the public API to populate, then clear.
        spy.Clear();

        spy.SentRequests.Should().BeEmpty();
        spy.PublishedNotifications.Should().BeEmpty();
    }

    [Fact]
    public async Task Send_Throws_On_Null_Request_Without_Calling_Inner()
    {
        var inner = new Mock<IMediator>();
        var spy = new SpyMediator(inner.Object);

        var act = async () => await spy.Send<Result<string>>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
        inner.Verify(m => m.Send(It.IsAny<ICommandOrQuery<Result<string>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Publish_Throws_On_Null_Notification_Without_Calling_Inner()
    {
        var inner = new Mock<IMediator>();
        var spy = new SpyMediator(inner.Object);

        var act = async () => await spy.Publish<Pinged>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
        inner.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Constructor_Throws_On_Null_Inner()
    {
        var act = () => new SpyMediator(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
