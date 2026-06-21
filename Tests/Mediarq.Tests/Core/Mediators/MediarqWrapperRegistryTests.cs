using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Moq;

namespace Mediarq.Tests.Core.Mediators;

public class MediarqWrapperRegistryTests
{
    public record RegPing(string Text) : ICommand<Result<string>>;

    public record RegEvent(int Id) : INotification;

    [Fact]
    public void Add_And_AddNotification_Are_Fluent()
    {
        var registry = new MediarqWrapperRegistry();

        registry.Add<RegPing, Result<string>>().Should().BeSameAs(registry);
        registry.AddNotification<RegEvent>().Should().BeSameAs(registry);
    }

    [Fact]
    public async Task Mediator_Dispatches_Send_Through_Supplied_Registry()
    {
        var registry = new MediarqWrapperRegistry().Add<RegPing, Result<string>>();

        var handler = new Mock<IRequestHandler<RegPing, Result<string>>>();
        handler.Setup(h => h.Handle(It.IsAny<RegPing>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success("ok"));

        var resolver = new Mock<IHandlerResolver>();
        resolver.Setup(r => r.Resolve<IRequestHandler<RegPing, Result<string>>>()).Returns(handler.Object);
        resolver.Setup(r => r.ResolveAll<IPipelineBehavior<RegPing, Result<string>>>()).Returns([]);

        var request = new RegPing("x");

        var factory = new Mock<IRequestContextFactory>();

        var mediator = new Mediator(factory.Object, resolver.Object, new ParallelNotificationPublisher(), registry);

        var result = await mediator.Send(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
        handler.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Mediator_Dispatches_Publish_Through_Supplied_Registry()
    {
        var registry = new MediarqWrapperRegistry().AddNotification<RegEvent>();

        var handler = new Mock<INotificationHandler<RegEvent>>();
        handler.Setup(h => h.Handle(It.IsAny<RegEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var resolver = new Mock<IHandlerResolver>();
        resolver.Setup(r => r.ResolveAll<INotificationHandler<RegEvent>>())
                .Returns(new[] { handler.Object });

        var mediator = new Mediator(
            Mock.Of<IRequestContextFactory>(),
            resolver.Object,
            new ParallelNotificationPublisher(),
            registry);

        var notification = new RegEvent(7);
        await mediator.Publish(notification);

        handler.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }
}
