
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
            .Setup(b => b.Handle(It.IsAny<IMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                 It.IsAny<Func<Task<Result<string>>>>(),
                                 It.IsAny<CancellationToken>()))
            .Returns(async (IMutableRequestContext<TestCommandWithValue, Result<string>> ctx, Func<Task<Result<string>>> next, CancellationToken _) =>
            {
                log.Add("Before 1");
                var result = await next();
                log.Add("After 1");
                return result;
            });

        // Behavior 2
        mockBehavior2
            .Setup(b => b.Handle(It.IsAny<IMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                 It.IsAny<Func<Task<Result<string>>>>(),
                                 It.IsAny<CancellationToken>()))
            .Returns(async (IMutableRequestContext<TestCommandWithValue, Result<string>> ctx, Func<Task<Result<string>>> next, CancellationToken _) =>
            {
                log.Add("Before 2");
                var result = await next();
                log.Add("After 2");
                return result;
            });

        // Behavior 3
        mockBehavior3
            .Setup(b => b.Handle(It.IsAny<IMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                 It.IsAny<Func<Task<Result<string>>>>(),
                                 It.IsAny<CancellationToken>()))
            .Returns(async (IMutableRequestContext<TestCommandWithValue, Result<string>> ctx, Func<Task<Result<string>>> next, CancellationToken _) =>
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
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
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

        // V�rifie que chaque behavior a bien �t� appel� une seule fois
        mockBehavior1.Verify(b => b.Handle(It.IsAny<IMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                           It.IsAny<Func<Task<Result<string>>>>(),
                                           It.IsAny<CancellationToken>()), Times.Once);
        mockBehavior2.Verify(b => b.Handle(It.IsAny<IMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                           It.IsAny<Func<Task<Result<string>>>>(),
                                           It.IsAny<CancellationToken>()), Times.Once);
        mockBehavior3.Verify(b => b.Handle(It.IsAny<IMutableRequestContext<TestCommandWithValue, Result<string>>>(),
                                           It.IsAny<Func<Task<Result<string>>>>(),
                                           It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_When_Context_Is_Null()
    {
        // Arrange
        var mockResolver = new Mock<IHandlerResolver>();
        mockResolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(Array.Empty<IPipelineBehavior<TestCommandWithValue, Result<string>>>());
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
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(Array.Empty<IPipelineBehavior<TestCommandWithValue, Result<string>>>());

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
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(Array.Empty<IPipelineBehavior<TestCommandWithValue, Result<string>>>());

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

    [Fact]
    public async Task ExecuteAsync_Should_Order_Behaviors_By_IOrderBehavior()
    {
        // Arrange — registered in descending Order on purpose (Order 2 before Order 1).
        var log = new List<string>();
        var behaviors = new IPipelineBehavior<TestCommandWithValue, Result<string>>[]
        {
            new OrderedBehavior(2, "B2", log),
            new OrderedBehavior(1, "B1", log),
        };

        var resolver = new Mock<IHandlerResolver>();
        resolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(behaviors);

        var executor = new PipelineExecutor(resolver.Object);
        var context = new RequestContext<TestCommandWithValue, Result<string>>(new TestCommandWithValue(""), "user");

        // Act
        await executor.ExecuteAsync(context, _ => Task.FromResult(Result.Success("OK")));

        // Assert — execution follows Order ascending, not registration order.
        log.Should().ContainInOrder("Before B1", "Before B2", "After B2", "After B1");
    }

    private sealed class OrderedBehavior(int order, string name, List<string> log)
        : IPipelineBehavior<TestCommandWithValue, Result<string>>, IOrderBehavior
    {
        public int Order { get; } = order;

        public async Task<Result<string>> Handle(
            IMutableRequestContext<TestCommandWithValue, Result<string>> context,
            Func<Task<Result<string>>> handle,
            CancellationToken cancellationToken = default)
        {
            log.Add($"Before {name}");
            var result = await handle();
            log.Add($"After {name}");
            return result;
        }
    }
}