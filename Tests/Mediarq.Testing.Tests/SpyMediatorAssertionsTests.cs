using FluentAssertions;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Testing.Tests.Fixtures;
using Moq;

namespace Mediarq.Testing.Tests;

public class SpyMediatorAssertionsTests
{
    [Fact]
    public async Task Sent_And_HasSent_Filter_By_Request_Type()
    {
        var inner = new Mock<IMediator>();
        inner.Setup(m => m.Send(It.IsAny<PingCommand>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(Result.Success("x"));
        inner.Setup(m => m.Send(It.IsAny<VoidCommand>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var spy = new SpyMediator(inner.Object);
        await spy.Send(new VoidCommand());
        await spy.Send<Result<string>>(new PingCommand("a"));
        await spy.Send<Result<string>>(new PingCommand("b"));

        spy.HasSent<PingCommand>().Should().BeTrue();
        spy.Sent<PingCommand>().Should().HaveCount(2);
        spy.Sent<VoidCommand>().Should().ContainSingle();
        spy.HasSent<Pinged>().Should().BeFalse();
    }

    [Fact]
    public async Task Published_And_HasPublished_Filter_By_Notification_Type()
    {
        var inner = new Mock<IMediator>();
        inner.Setup(m => m.Publish(It.IsAny<Pinged>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var spy = new SpyMediator(inner.Object);
        await spy.Publish(new Pinged(1));
        await spy.Publish(new Pinged(2));

        spy.HasPublished<Pinged>().Should().BeTrue();
        spy.Published<Pinged>().Select(p => p.Id).Should().Equal(1, 2);
    }
}
