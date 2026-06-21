
using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Moq;

namespace Mediarq.Tests.Core.Mediators;
public class MediatorTests
{
    private readonly Mediator _testClass;
    private readonly Mock<IHandlerResolver> _mockHandlerResolver;
    private readonly Mock<IRequestContextFactory> _mockRequestContextFactory;
    private readonly Mock<IRequestHandler<TestCommand, Result<string>>> _mockHandler;
    private readonly INotificationPublisher _publisher = new ParallelNotificationPublisher();

    public record TestCommand(string Message) : ICommand<Result<string>>;

    public MediatorTests()
    {
        _mockHandlerResolver = new Mock<IHandlerResolver>();
        _mockRequestContextFactory = new Mock<IRequestContextFactory>();
        _mockHandler = new Mock<IRequestHandler<TestCommand, Result<string>>>();

        // Default resolver: returns a valid handler and no behaviors.
        _mockHandlerResolver
            .Setup(r => r.Resolve<IRequestHandler<TestCommand, Result<string>>>())
            .Returns(_mockHandler.Object);
        _mockHandlerResolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommand, Result<string>>>())
            .Returns([]);

        _testClass = new Mediator(
            _mockRequestContextFactory.Object,
            _mockHandlerResolver.Object,
            _publisher);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new Mediator(
            _mockRequestContextFactory.Object,
            _mockHandlerResolver.Object,
            _publisher);

        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullHandlerResolver()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            null!,
            _publisher);

        act.Should().Throw<ArgumentNullException>().WithParameterName("handlerResolver");
    }

    [Fact]
    public void CannotConstructWithNullRequestContextFactory()
    {
        Action act = () => new Mediator(
            null!,
            _mockHandlerResolver.Object,
            _publisher);

        act.Should().Throw<ArgumentNullException>().WithParameterName("requestContextFactory");
    }

    [Fact]
    public void CannotConstructWithNullNotificationPublisher()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            _mockHandlerResolver.Object,
            null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("notificationPublisher");
    }

    [Fact]
    public async Task Send_ShouldThrow_WhenRequestIsNull()
    {
        await FluentActions
            .Invoking(() => _testClass.Send<string>(null!, CancellationToken.None))
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task Send_ShouldThrow_WhenHandlerNotFound()
    {
        // Arrange
        _mockHandlerResolver
            .Setup(r => r.Resolve<IRequestHandler<TestCommand, Result<string>>>())
            .Returns((IRequestHandler<TestCommand, Result<string>>)null);

        var request = new TestCommand("Hello");

        // Act + Assert
        await FluentActions
            .Invoking(() => _testClass.Send(request, CancellationToken.None))
            .Should()
            .ThrowAsync<HandlerNotFoundException>()
            .WithMessage("No handler found*");
    }

    [Fact]
    public async Task Send_ShouldInvokeHandler_WhenNoBehaviors()
    {
        // Arrange
        var request = new TestCommand("Hello");
        var expected = Result.Success("OK");

        _mockHandler
            .Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act — no behaviors registered, so the mediator dispatches straight to the handler and never
        // creates a request context.
        var result = await _testClass.Send(request, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
        _mockHandlerResolver.Verify(r => r.Resolve<IRequestHandler<TestCommand, Result<string>>>(), Times.Once);
        _mockHandler.Verify(h => h.Handle(request, It.IsAny<CancellationToken>()), Times.Once);
        _mockRequestContextFactory.Verify(
            f => f.Create<TestCommand, Result<string>>(It.IsAny<TestCommand>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Send_ShouldWrapExceptionsInInvalidOperationException()
    {
        // Arrange — a synchronous failure while dispatching is wrapped by the mediator.
        var request = new TestCommand("Fail");

        _mockHandlerResolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommand, Result<string>>>())
            .Throws(new Exception("Failure while resolving behaviors"));

        // Act + Assert
        await FluentActions
            .Invoking(() => _testClass.Send(request, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Error while handling request*");
    }
}
