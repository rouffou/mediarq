using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Mediators;
using Moq;

namespace Mediarq.Tests.Core.Mediators;

public class MediatorNotificationTests
{
    public record SampleNotification(string Message) : INotification;

    private readonly Mock<IHandlerResolver> _resolver = new();
    private readonly Mediator _mediator;

    public MediatorNotificationTests()
    {
        _mediator = new Mediator(
            Mock.Of<IRequestContextFactory>(),
            Mock.Of<IPipelineExecutor>(),
            _resolver.Object,
            new ParallelNotificationPublisher());
    }

    [Fact]
    public async Task Publish_ShouldThrow_WhenNotificationIsNull()
    {
        await FluentActions
            .Invoking(() => _mediator.Publish<SampleNotification>(null!))
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("notification");
    }

    [Fact]
    public async Task Publish_ShouldInvokeAllHandlers()
    {
        var handler1 = new Mock<INotificationHandler<SampleNotification>>();
        var handler2 = new Mock<INotificationHandler<SampleNotification>>();
        handler1.Setup(h => h.Handle(It.IsAny<SampleNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        handler2.Setup(h => h.Handle(It.IsAny<SampleNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _resolver
            .Setup(r => r.ResolveAll(typeof(INotificationHandler<SampleNotification>)))
            .Returns(new object[] { handler1.Object, handler2.Object });

        var notification = new SampleNotification("hello");

        await _mediator.Publish(notification);

        handler1.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
        handler2.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_ShouldBeNoop_WhenNoHandlersRegistered()
    {
        _resolver
            .Setup(r => r.ResolveAll(It.IsAny<Type>()))
            .Returns(Array.Empty<object>());

        var act = async () => await _mediator.Publish(new SampleNotification("x"));

        await act.Should().NotThrowAsync();
    }
}
