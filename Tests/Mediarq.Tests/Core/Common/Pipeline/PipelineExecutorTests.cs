
using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Results;
using Mediarq.Tests.Data;
using Moq;

namespace Mediarq.Tests.Core.Common.Pipeline;
public class PipelineExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_Should_Invoke_Behaviors_In_Correct_Order()
    {
        // Arrange
        var log = new List<string>();

        var mockBehavior1 = new Mock<IPipelineBehavior<TestCommandWithValue, Result<string>>>();
        var mockBehavior2 = new Mock<IPipelineBehavior<TestCommandWithValue, Result<string>>>();
        var mockBehavior3 = new Mock<IPipelineBehavior<TestCommandWithValue, Result<string>>>();

        // Behavior 1
        mockBehavior1
            .Setup(b => b.Handle(It.IsAny<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                 It.IsAny<Func<Task<Result<string>>>>(),
                                 It.IsAny<CancellationToken>()))
            .Returns(async (IIMMutableRequestContext<TestCommandWithValue, Result<string>> ctx, Func<Task<Result<string>>> next, CancellationToken _) =>
            {
                log.Add("Before 1");
                var result = await next();
                log.Add("After 1");
                return result;
            });

        // Behavior 2
        mockBehavior2
            .Setup(b => b.Handle(It.IsAny<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                 It.IsAny<Func<Task<Result<string>>>>(),
                                 It.IsAny<CancellationToken>()))
            .Returns(async (IIMMutableRequestContext<TestCommandWithValue, Result<string>> ctx, Func<Task<Result<string>>> next, CancellationToken _) =>
            {
                log.Add("Before 2");
                var result = await next();
                log.Add("After 2");
                return result;
            });

        // Behavior 3
        mockBehavior3
            .Setup(b => b.Handle(It.IsAny<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                 It.IsAny<Func<Task<Result<string>>>>(),
                                 It.IsAny<CancellationToken>()))
            .Returns(async (IIMMutableRequestContext<TestCommandWithValue, Result<string>> ctx, Func<Task<Result<string>>> next, CancellationToken _) =>
            {
                log.Add("Before 3");
                var result = await next();
                log.Add("After 3");
                return result;
            });

        var behaviors = new[]
        {
            mockBehavior1.Object,
            mockBehavior2.Object,
            mockBehavior3.Object
        };

        // Simule la factory qui renvoie nos mocks
        var mockResolver = new Mock<IHandlerResolver>();
        mockResolver
            .Setup(r => r.Resolve(typeof(IEnumerable<IPipelineBehavior<TestCommandWithValue, Result<string>>>)))
            .Returns(behaviors);

        var executor = new PipelineExecutor(mockResolver.Object);
        var context = new RequestContext<TestCommandWithValue, Result<string>>(new TestCommandWithValue(""), Guid.NewGuid().ToString());

        // Act
        var result = await executor.ExecuteAsync(context, _ =>
        {
            log.Add("Handler");
            return Task.FromResult(Result.Success("OK"));
        });

        // Assert
        result.Value.Should().Be("OK");

        log.Should().ContainInOrder(
            "Before 1",
            "Before 2",
            "Before 3",
            "Handler",
            "After 3",
            "After 2",
            "After 1"
        );

        // Vérifie que chaque behavior a bien été appelé une seule fois
        mockBehavior1.Verify(b => b.Handle(It.IsAny<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                           It.IsAny<Func<Task<Result<string>>>>(),
                                           It.IsAny<CancellationToken>()), Times.Once);
        mockBehavior2.Verify(b => b.Handle(It.IsAny<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                           It.IsAny<Func<Task<Result<string>>>>(),
                                           It.IsAny<CancellationToken>()), Times.Once);
        mockBehavior3.Verify(b => b.Handle(It.IsAny<IIMMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                           It.IsAny<Func<Task<Result<string>>>>(),
                                           It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Context_Is_Null()
    {
        // Arrange
        var mockResolver = new Mock<IHandlerResolver>();
        mockResolver
            .Setup(r => r.Resolve(typeof(IEnumerable<IPipelineBehavior<TestCommandWithValue, Result<string>>>)))
            .Returns(Enumerable.Empty<IPipelineBehavior<TestCommandWithValue, Result<string>>>());
        var executor = new PipelineExecutor(mockResolver.Object);

        // Act
        var act = async () =>
            await executor.ExecuteAsync<TestCommandWithValue, Result<string>>(
                null!,
                _ => Task.FromResult(Result.Success("OK")));

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("context");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_HandlerDelegate_Is_Null()
    {
        // Arrange
        var mockResolver = new Mock<IHandlerResolver>();
        mockResolver
            .Setup(r => r.Resolve(typeof(IEnumerable<IPipelineBehavior<TestCommandWithValue, Result<string>>>)))
            .Returns(Enumerable.Empty<IPipelineBehavior<TestCommandWithValue, Result<string>>>());

        var executor = new PipelineExecutor(mockResolver.Object);
        var context = new RequestContext<TestCommandWithValue, Result<string>>(new TestCommandWithValue(""), Guid.NewGuid().ToString());

        // Act
        var act = async () =>
            await executor.ExecuteAsync<TestCommandWithValue, Result<string>>(
                context,
                null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("handlerDelegate");
    }

    [Fact]
    public async Task ExecuteAsync_Should_Invoke_Handler_When_No_Behaviors()
    {
        // Arrange
        bool handlerCalled = false; var mockResolver = new Mock<IHandlerResolver>();
        
        mockResolver
            .Setup(r => r.Resolve(typeof(IEnumerable<IPipelineBehavior<TestCommandWithValue, Result<string>>>)))
            .Returns(Enumerable.Empty<IPipelineBehavior<TestCommandWithValue, Result<string>>>());

        var executor = new PipelineExecutor(mockResolver.Object);
        var context = new RequestContext<TestCommandWithValue, Result<string>>(new TestCommandWithValue(""), Guid.NewGuid().ToString());

        // Act
        var result = await executor.ExecuteAsync(context, _ =>
        {
            handlerCalled = true;
            return Task.FromResult(Result.Success("OK"));
        });

        // Assert
        handlerCalled.Should().BeTrue();
        result.Value.Should().Be("OK");
    }
}