
using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
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

    public record TestCommand(string Message) : ICommand<Result<string>>;

    public MediatorTests()
    {
        _mockHandlerResolver = new Mock<IHandlerResolver>();
        _mockRequestContextFactory = new Mock<IRequestContextFactory>();
        _mockPipelineExecutor = new Mock<IPipelineExecutor>();
        _mockHandler = new Mock<IRequestHandler<TestCommand, Result<string>>>();

        // Default resolver: returns a valid handler
        _mockHandlerResolver
            .Setup(r => r.Resolve(typeof(IRequestHandler<TestCommand, Result<string>>)))
            .Returns(_mockHandler.Object);

        _testClass = new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
            _mockHandlerResolver.Object);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
            _mockHandlerResolver.Object);

        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullHandlerResolver()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            _mockPipelineExecutor.Object,
            null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("handlerResolver");
    }

    [Fact]
    public void CannotConstructWithNullRequestContextFactory()
    {
        Action act = () => new Mediator(
            null!,
            _mockPipelineExecutor.Object,
            _mockHandlerResolver.Object);

        act.Should().Throw<ArgumentNullException>().WithParameterName("requestContextFactory");
    }

    [Fact]
    public void CannotConstructWithNullPipelineExecutor()
    {
        Action act = () => new Mediator(
            _mockRequestContextFactory.Object,
            null!,
            _mockHandlerResolver.Object);

        act.Should().Throw<ArgumentNullException>().WithParameterName("pipelineExecutor");
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
            .Setup(r => r.Resolve(typeof(IRequestHandler<TestCommand, Result<string>>)))
            .Returns(null);

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
        var context = new RequestContext<TestCommand, Result<string>>(request, Guid.NewGuid().ToString(), CancellationToken.None);
        var expected = Result.Success("OK");

        // Mock handler behavior
        _mockHandler
            .Setup(h => h.Handle(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("OK"));

        _mockHandlerResolver
            .Setup(r => r.Resolve(typeof(IRequestHandler<TestCommand, Result<string>>)))
            .Returns(_mockHandler.Object);

        _mockRequestContextFactory
            .Setup(f => f.Create<TestCommand, Result<string>>(request, It.IsAny<CancellationToken>()))
            .Returns(context);

        _mockPipelineExecutor
            .Setup(p => p.ExecuteAsync(
                context,
                It.IsAny<Func<CancellationToken, Task<Result<string>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _testClass.Send(request, CancellationToken.None);

        // Assert
        result.Should().Be(expected);

        _mockHandlerResolver.Verify(r => r.Resolve(typeof(IRequestHandler<TestCommand, Result<string>>)), Times.Once);
        _mockRequestContextFactory.Verify(f => f.Create<TestCommand, Result<string>>(request, It.IsAny<CancellationToken>()), Times.Once);
        _mockPipelineExecutor.Verify(p => p.ExecuteAsync(
            context,
            It.IsAny<Func<CancellationToken, Task<Result<string>>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Send_ShouldWrapExceptionsInInvalidOperationException()
    {
        // Arrange
        var request = new TestCommand("Fail");

        _mockRequestContextFactory
            .Setup(f => f.Create<TestCommand, Result<string>>(request, It.IsAny<CancellationToken>()))
            .Throws(new Exception("Failure in factory"));

        // Act + Assert
        await FluentActions
            .Invoking(() => _testClass.Send(request, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Error while handling request*");
    }
}