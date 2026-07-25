using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Mediators;
using Moq;

namespace Mediarq.Tests.Core.Mediators;

public class PolymorphicNotificationTests
{
    public abstract record GrandparentEvent : INotification;

    public abstract record ParentEvent : GrandparentEvent;

    public sealed record LeafEvent(string Message) : ParentEvent, IPolymorphicNotification;

    public sealed record NonPolymorphicLeafEvent(string Message) : ParentEvent;

    // Implements IPolymorphicNotification directly with no intermediate notification base type
    // (its only base is object), so the hierarchy walk in BuildPolymorphicHandlerServiceTypes finds
    // nothing to resolve.
    public sealed record TopLevelPolymorphicEvent(string Message) : INotification, IPolymorphicNotification;

    private readonly Mock<IHandlerResolver> _resolver = new();
    private readonly Mediator _mediator;

    public PolymorphicNotificationTests()
    {
        _mediator = new Mediator(
            Mock.Of<IRequestContextFactory>(),
            _resolver.Object,
            new SequentialNotificationPublisher());
    }

    private void SetupConcrete<TNotification>(params INotificationHandler<TNotification>[] handlers)
        where TNotification : INotification
        => _resolver.Setup(r => r.ResolveAll<INotificationHandler<TNotification>>()).Returns(handlers);

    private void SetupBaseType(Type baseType, params object[] handlers)
        => _resolver.Setup(r => r.ResolveAll(typeof(INotificationHandler<>).MakeGenericType(baseType))).Returns(handlers);

    [Fact]
    public async Task Publish_Dispatches_To_A_Base_Type_Handler_For_A_Polymorphic_Notification()
    {
        var baseHandler = new Mock<INotificationHandler<ParentEvent>>();
        baseHandler.Setup(h => h.Handle(It.IsAny<ParentEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        SetupConcrete<LeafEvent>();
        SetupBaseType(typeof(ParentEvent), baseHandler.Object);
        SetupBaseType(typeof(GrandparentEvent));

        var notification = new LeafEvent("hello");
        await _mediator.Publish(notification);

        baseHandler.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_Does_Not_Query_Base_Type_Handlers_For_A_Non_Polymorphic_Notification()
    {
        SetupConcrete<NonPolymorphicLeafEvent>();

        await _mediator.Publish(new NonPolymorphicLeafEvent("hello"));

        _resolver.Verify(r => r.ResolveAll(It.IsAny<Type>()), Times.Never);
    }

    [Fact]
    public async Task Publish_Runs_The_Concrete_Handler_Before_Base_Type_Handlers_By_Default()
    {
        var log = new List<string>();
        var concreteHandler = new LoggingHandler<LeafEvent>("concrete", log);
        var baseHandler = new LoggingBaseHandler<ParentEvent>("base", log);

        SetupConcrete<LeafEvent>(concreteHandler);
        SetupBaseType(typeof(ParentEvent), baseHandler);
        SetupBaseType(typeof(GrandparentEvent));

        await _mediator.Publish(new LeafEvent("x"));

        log.Should().Equal("concrete", "base");
    }

    [Fact]
    public async Task Publish_Dispatches_To_Every_Base_Type_In_The_Hierarchy_Most_Specific_First()
    {
        var log = new List<string>();
        var parentHandler = new LoggingBaseHandler<ParentEvent>("parent", log);
        var grandparentHandler = new LoggingBaseHandler<GrandparentEvent>("grandparent", log);

        SetupConcrete<LeafEvent>();
        SetupBaseType(typeof(ParentEvent), parentHandler);
        SetupBaseType(typeof(GrandparentEvent), grandparentHandler);

        await _mediator.Publish(new LeafEvent("x"));

        log.Should().Equal("parent", "grandparent");
    }

    [Fact]
    public async Task Publish_Honors_Explicit_Order_Across_Concrete_And_Base_Type_Handlers()
    {
        var log = new List<int>();
        var concreteHandler = new OrderedConcreteHandler(2, log);
        var baseHandler = new OrderedBaseHandler(1, log);

        SetupConcrete<LeafEvent>(concreteHandler);
        SetupBaseType(typeof(ParentEvent), baseHandler);
        SetupBaseType(typeof(GrandparentEvent));

        await _mediator.Publish(new LeafEvent("x"));

        // The base-type handler explicitly orders itself first (Order 1), overriding the
        // concrete-handlers-run-first default, because at least one handler opts into ordering.
        log.Should().Equal(1, 2);
    }

    [Fact]
    public async Task Publish_Honors_Explicit_Order_When_Only_The_Base_Type_Handler_Is_Ordered()
    {
        var log = new List<string>();
        var concreteHandler = new LoggingHandler<LeafEvent>("concrete", log);
        var baseHandler = new OrderedLoggingBaseHandler(1, "base", log);

        SetupConcrete<LeafEvent>(concreteHandler);
        SetupBaseType(typeof(ParentEvent), baseHandler);
        SetupBaseType(typeof(GrandparentEvent));

        await _mediator.Publish(new LeafEvent("x"));

        // The concrete handler isn't ordered, so hasOrdered is only discovered while scanning the
        // base-type (polymorphic) handlers — exercising that scan in isolation. Once found, the
        // explicitly-ordered base handler (Order 1) still runs before the unordered concrete one.
        log.Should().Equal("base", "concrete");
    }

    [Fact]
    public async Task Publish_Does_Not_Resolve_Any_Base_Type_When_The_Notification_Has_No_Notification_Base_Type()
    {
        SetupConcrete<TopLevelPolymorphicEvent>();

        var act = async () => await _mediator.Publish(new TopLevelPolymorphicEvent("x"));

        await act.Should().NotThrowAsync();
        _resolver.Verify(r => r.ResolveAll(It.IsAny<Type>()), Times.Never);
    }

    [Fact]
    public async Task Publish_Is_Noop_When_No_Handler_Is_Registered_At_Any_Tier()
    {
        SetupConcrete<LeafEvent>();
        SetupBaseType(typeof(ParentEvent));
        SetupBaseType(typeof(GrandparentEvent));

        var act = async () => await _mediator.Publish(new LeafEvent("x"));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Publish_Takes_The_Single_Handler_Fast_Path_When_Only_A_Base_Type_Handler_Exists()
    {
        var baseHandler = new Mock<INotificationHandler<ParentEvent>>();
        baseHandler.Setup(h => h.Handle(It.IsAny<ParentEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        SetupConcrete<LeafEvent>();
        SetupBaseType(typeof(ParentEvent), baseHandler.Object);
        SetupBaseType(typeof(GrandparentEvent));

        var notification = new LeafEvent("solo");
        await _mediator.Publish(notification);

        baseHandler.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    // INotificationHandler<in TNotification> is contravariant: a handler registered for ParentEvent
    // (or GrandparentEvent) genuinely implements INotificationHandler<LeafEvent> too, so it can log
    // straight from its own Handle(ParentEvent, ...) overload without any cast.
    private sealed class LoggingHandler<TNotification>(string tag, List<string> log) : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        public Task Handle(TNotification notification, CancellationToken cancellationToken = default)
        {
            log.Add(tag);
            return Task.CompletedTask;
        }
    }

    private sealed class LoggingBaseHandler<TBase>(string tag, List<string> log) : INotificationHandler<TBase>
        where TBase : INotification
    {
        public Task Handle(TBase notification, CancellationToken cancellationToken = default)
        {
            log.Add(tag);
            return Task.CompletedTask;
        }
    }

    private sealed class OrderedConcreteHandler(int order, List<int> log) : INotificationHandler<LeafEvent>, IOrderedNotificationHandler
    {
        public int Order => order;

        public Task Handle(LeafEvent notification, CancellationToken cancellationToken = default)
        {
            log.Add(order);
            return Task.CompletedTask;
        }
    }

    private sealed class OrderedLoggingBaseHandler(int order, string tag, List<string> log) : INotificationHandler<ParentEvent>, IOrderedNotificationHandler
    {
        public int Order => order;

        public Task Handle(ParentEvent notification, CancellationToken cancellationToken = default)
        {
            log.Add(tag);
            return Task.CompletedTask;
        }
    }

    private sealed class OrderedBaseHandler(int order, List<int> log) : INotificationHandler<ParentEvent>, IOrderedNotificationHandler
    {
        public int Order => order;

        public Task Handle(ParentEvent notification, CancellationToken cancellationToken = default)
        {
            log.Add(order);
            return Task.CompletedTask;
        }
    }
}
