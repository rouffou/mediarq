using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Validators;
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
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(new[] { handler1.Object, handler2.Object });

        var notification = new SampleNotification("hello");

        await _mediator.Publish(notification);

        handler1.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
        handler2.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_ShouldBeNoop_WhenNoHandlersRegistered()
    {
        _resolver
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(Array.Empty<INotificationHandler<SampleNotification>>());

        var act = async () => await _mediator.Publish(new SampleNotification("x"));

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Publish_Invokes_The_Single_Handler()
    {
        var handler = new Mock<INotificationHandler<SampleNotification>>();
        handler.Setup(h => h.Handle(It.IsAny<SampleNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _resolver
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(new[] { handler.Object });

        var notification = new SampleNotification("solo");

        await _mediator.Publish(notification);

        handler.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_Runs_Ordered_Handlers_First_Then_Unordered_In_Registration_Order()
    {
        var log = new List<int>();
        // Mix: an unordered handler, then two ordered (registered 2 before 1). Ordered run by Order
        // (1, 2); the unordered one defaults to int.MaxValue and runs last.
        _resolver
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(new INotificationHandler<SampleNotification>[]
            {
                new UnorderedHandler(99, log),
                new OrderedHandler(2, log),
                new OrderedHandler(1, log),
            });

        var mediator = new Mediator(
            Mock.Of<IRequestContextFactory>(),
            _resolver.Object,
            new SequentialNotificationPublisher());

        await mediator.Publish(new SampleNotification("x"));

        log.Should().Equal(1, 2, 99);
    }

    [Fact]
    public async Task Publish_Invokes_Handlers_By_Ascending_Order()
    {
        var log = new List<int>();
        // Registered in reverse order on purpose (Order 2 before Order 1).
        _resolver
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(new INotificationHandler<SampleNotification>[] { new OrderedHandler(2, log), new OrderedHandler(1, log) });

        var mediator = new Mediator(
            Mock.Of<IRequestContextFactory>(),
            _resolver.Object,
            new SequentialNotificationPublisher());

        await mediator.Publish(new SampleNotification("x"));

        log.Should().Equal(1, 2);
    }

    [Fact]
    public async Task Publish_Runs_Handlers_When_Notification_Is_Valid()
    {
        var handler = new Mock<INotificationHandler<SampleNotification>>();
        handler.Setup(h => h.Handle(It.IsAny<SampleNotification>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _resolver
            .Setup(r => r.ResolveAll<IValidator<SampleNotification>>())
            .Returns(new IValidator<SampleNotification>[] { new SampleNotificationValidator() });
        _resolver
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(new[] { handler.Object });

        var notification = new SampleNotification("ok");

        await _mediator.Publish(notification);

        handler.Verify(h => h.Handle(notification, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Publish_Throws_And_Skips_Handlers_When_Notification_Is_Invalid()
    {
        var handler = new Mock<INotificationHandler<SampleNotification>>();

        _resolver
            .Setup(r => r.ResolveAll<IValidator<SampleNotification>>())
            .Returns(new IValidator<SampleNotification>[] { new SampleNotificationValidator() });
        _resolver
            .Setup(r => r.ResolveAll<INotificationHandler<SampleNotification>>())
            .Returns(new[] { handler.Object });

        // Empty message fails the validator.
        var act = () => _mediator.Publish(new SampleNotification(""));

        (await act.Should().ThrowAsync<NotificationValidationException>())
            .Which.Errors.Should().ContainSingle(e => e.PropertyName == nameof(SampleNotification.Message));

        handler.Verify(h => h.Handle(It.IsAny<SampleNotification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private sealed class SampleNotificationValidator : IValidator<SampleNotification>
    {
        public IEnumerable<ValidationResult> Validate(SampleNotification instance)
        {
            if (string.IsNullOrWhiteSpace(instance.Message))
            {
                yield return ValidationResult.Failure([new ValidationPropertyError(nameof(SampleNotification.Message), "Message is required.")]);
            }
            else
            {
                yield return ValidationResult.Success();
            }
        }
    }

    private sealed class OrderedHandler(int order, List<int> log) : INotificationHandler<SampleNotification>, IOrderedNotificationHandler
    {
        public int Order => order;

        public Task Handle(SampleNotification notification, CancellationToken cancellationToken = default)
        {
            log.Add(order);
            return Task.CompletedTask;
        }
    }

    private sealed class UnorderedHandler(int tag, List<int> log) : INotificationHandler<SampleNotification>
    {
        public Task Handle(SampleNotification notification, CancellationToken cancellationToken = default)
        {
            log.Add(tag);
            return Task.CompletedTask;
        }
    }
}
