
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
    private readonly Mock<IPipelineExecutor> _mockPipelineExecutor;
    private readonly Mock<IRequestHandler<TestCommand, Result<string>>> _mockHandler;
    private readonly INotificationPublisher _publisher = new ParallelNotificationPublisher();

    public record TestCommand(string Message) : ICommand<Result<string>>;

    public MediatorTests()
    {
        _mockHandlerResolver = new Mock<IHandlerResolver>();
        _mockRequestContextFactory = new Mock<IRequestContextFactory>();
        _mockPipelineExecutor = new Mock<IPipelineExecutor>();
        _mockHandler = new Mock<IRequestHandler<TestCommand, Result<string>>>();

        // Default resolver: returns a valid handler
        _mockHandlerResolver
            .Setup(r => r.Resolve<IRequestHandler<TestCommand, Result<string>>>())
            .Returns(_mockHandler.Object);

        _testClass = new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
            _mockHandlerResolver.Object,
            _publisher);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
            _mockHandlerResolver.Object,
            _publisher);

        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullHandlerResolver()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
            null!,
            _publisher);

        act.Should().Throw<ArgumentNullException>().WithParameterName("handlerResolver");
    }

    [Fact]
    public void CannotConstructWithNullRequestContextFactory()
    {
        Action act = () => new Mediator(
            null!,
            _mockPipelineExecutor.Object,
            _mockHandlerResolver.Object,
            _publisher);

        act.Should().Throw<ArgumentNullException>().WithParameterName("requestContextFactory");
    }

    [Fact]
    public void CannotConstructWithNullPipelineExecutor()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            null!,
            _mockHandlerResolver.Object,
            _publisher);

        act.Should().Throw<ArgumentNullException>().WithParameterName("pipelineExecutor");
    }

    [Fact]
    public void CannotConstructWithNullNotificationPublisher()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
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
    public async Task Send_ShouldCallPipelineExecutor_WhenHandlerExists()
    {
        // Arrange
        var request = new TestCommand("Hello");
        var expected = Result.Success("OK");

        _mockHandlerResolver
            .Setup(r => r.Resolve<IRequestHandler<TestCommand, Result<string>>>())
            .Returns(_mockHandler.Object);

        // The mediator now hands the resolved handler and the context factory to the executor, which
        // creates the context lazily — so the executor is invoked through the request/handler overload.
        _mockPipelineExecutor
            .Setup(p => p.ExecuteAsync(
                request,
                _mockHandler.Object,
                _mockRequestContextFactory.Object,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _testClass.Send(request, CancellationToken.None);

        // Assert
        result.Should().Be(expected);

        _mockHandlerResolver.Verify(r => r.Resolve<IRequestHandler<TestCommand, Result<string>>>(), Times.Once);
        _mockPipelineExecutor.Verify(p => p.ExecuteAsync(
            request,
            _mockHandler.Object,
            _mockRequestContextFactory.Object,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Send_ShouldWrapExceptionsInInvalidOperationException()
    {
        // Arrange
        var request = new TestCommand("Fail");

        _mockPipelineExecutor
            .Setup(p => p.ExecuteAsync(
                request,
                It.IsAny<IRequestHandler<TestCommand, Result<string>>>(),
                It.IsAny<IRequestContextFactory>(),
                It.IsAny<CancellationToken>()))
            .Throws(new Exception("Failure in executor"));

        // Act + Assert
        await FluentActions
            .Invoking(() => _testClass.Send(request, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Error while handling request*");
    }
}