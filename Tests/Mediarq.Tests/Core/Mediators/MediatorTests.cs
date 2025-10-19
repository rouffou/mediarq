
using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Moq;

namespace Mediarq.Tests.Core.Mediators;
public class MediatorTests
{
    private readonly Mediator _testClass;
    private readonly Mock<IRequestContextFactory> _mockRequestContextFactory;
    private readonly Mock<IPipelineExecutor> _mockPipelineExecutor;
    private readonly Mock<IRequestHandler<TestCommand, Result<string>>> _mockHandler;
    private readonly ServiceFactory _serviceFactory;

    public record TestCommand(string Message) : ICommand<Result<string>>;

    public MediatorTests()
    {
        _mockRequestContextFactory = new Mock<IRequestContextFactory>();
        _mockPipelineExecutor = new Mock<IPipelineExecutor>();
        _mockHandler = new Mock<IRequestHandler<TestCommand, Result<string>>>();

        // Par défaut, on renvoie un handler valide
        _serviceFactory = type =>
        {
            if (type == typeof(IRequestHandler<TestCommand, Result<string>>))
                return _mockHandler.Object;
            return null;
        };

        _testClass = new Mediator(_serviceFactory, _mockRequestContextFactory.Object, _mockPipelineExecutor.Object);
    }

    [Fact]
    public void CanConstruct()
    {
        var instance = new Mediator(_serviceFactory, _mockRequestContextFactory.Object, _mockPipelineExecutor.Object);
        instance.Should().NotBeNull();
    }

    [Fact]
    public void CannotConstructWithNullServiceFactory()
    {
        FluentActions.Invoking(() =>
            new Mediator(default(ServiceFactory), _mockRequestContextFactory.Object, _mockPipelineExecutor.Object))
            .Should().Throw<ArgumentNullException>().WithParameterName("serviceFactory");
    }

    [Fact]
    public void CannotConstructWithNullRequestContextFactory()
    {
        FluentActions.Invoking(() =>
            new Mediator(_serviceFactory, default(IRequestContextFactory), _mockPipelineExecutor.Object))
            .Should().Throw<ArgumentNullException>().WithParameterName("requestContextFactory");
    }

    [Fact]
    public void CannotConstructWithNullPipelineExecutor()
    {
        FluentActions.Invoking(() =>
            new Mediator(_serviceFactory, _mockRequestContextFactory.Object, default(IPipelineExecutor)))
            .Should().Throw<ArgumentNullException>().WithParameterName("pipelineExecutor");
    }

    [Fact]
    public async Task Send_ShouldThrow_WhenRequestIsNull()
    {
        await FluentActions.Invoking(() =>
            _testClass.Send<string>(null, CancellationToken.None))
            .Should().ThrowAsync<ArgumentNullException>().WithParameterName("request");
    }

    [Fact]
    public async Task Send_ShouldThrow_WhenHandlerNotFound()
    {
        // Arrange : factory qui ne retourne pas de handler
        var mediator = new Mediator(type => null, _mockRequestContextFactory.Object, _mockPipelineExecutor.Object);
        var request = new TestCommand("Hello");

        // Act / Assert
        await FluentActions.Invoking(() => mediator.Send(request, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("No handler found*");
    }

    [Fact]
    public async Task Send_ShouldCallPipelineExecutor_WhenHandlerExists()
    {
        // Arrange
        var request = new TestCommand("Hello");
        var context = new object();
        var expectedResponse = "Response OK";

        _mockRequestContextFactory
            .Setup(f => f.Create<TestCommand, Result<string>>(
                request, It.IsAny<CancellationToken>()))
            .Returns(context);

        
        _mockPipelineExecutor
            .Setup(p => p.ExecuteAsync<TestCommand, Result<string>>(
                It.IsAny<RequestContext<TestCommand, Result<string>>>(),
                It.IsAny<Func<CancellationToken, Task<Result<string>>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _testClass.Send(request, CancellationToken.None);

        // Assert
        result.Value.Should().Be(expectedResponse);

        _mockRequestContextFactory.Verify(f => f.Create<ICommandOrQuery<Result<string>>, Result<string>>(
            request, It.IsAny<CancellationToken>()), Times.Once);

        _mockPipelineExecutor.Verify(p => p.ExecuteAsync<TestCommand, Result<string>>(
            It.IsAny<RequestContext<TestCommand,Result<string>>>(),
            It.IsAny<Func<CancellationToken, Task<Result<string>>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Send_ShouldWrapExceptionsInInvalidOperationException()
    {
        // Arrange
        var request = new TestCommand("Fail");
        _mockRequestContextFactory
            .Setup(f => f.Create<ICommandOrQuery<Result<string>>, Result<string>>(request, It.IsAny<CancellationToken>()))
            .Throws(new Exception("Failure in factory"));

        // Act / Assert
        await FluentActions.Invoking(() => _testClass.Send(request, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Error while handling request*");
    }
}