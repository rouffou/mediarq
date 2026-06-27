
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

    [Fact]
    public async Task ExecuteAsync_Should_Skip_Inactive_Conditional_Behaviors()
    {
        // Arrange — one inactive conditional behavior and one active plain behavior.
        var log = new List<string>();
        var behaviors = new IPipelineBehavior<TestCommandWithValue, Result<string>>[]
        {
            new ConditionalBehavior(isActive: false, "Inactive", log),
            new OrderedBehavior(1, "Active", log),
        };

        var resolver = new Mock<IHandlerResolver>();
        resolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(behaviors);

        var executor = new PipelineExecutor(resolver.Object);
        var context = new RequestContext<TestCommandWithValue, Result<string>>(new TestCommandWithValue(""), "user");

        // Act
        await executor.ExecuteAsync(context, _ => Task.FromResult(Result.Success("OK")));

        // Assert — only the active behavior ran.
        log.Should().ContainInOrder("Before Active", "After Active");
        log.Should().NotContain(e => e.Contains("Inactive"));
    }

    [Fact]
    public async Task ExecuteAsync_LazyContext_Invokes_Handler_Directly_When_No_Active_Behavior()
    {
        // Arrange — one inactive conditional behavior, so nothing should run before the handler.
        var resolver = new Mock<IHandlerResolver>();
        resolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(new IPipelineBehavior<TestCommandWithValue, Result<string>>[] { new ConditionalBehavior(isActive: false, "Inactive", []) });

        var handler = new Mock<Mediarq.Core.Common.Requests.Abstraction.IRequestHandler<TestCommandWithValue, Result<string>>>();
        handler.Setup(h => h.Handle(It.IsAny<TestCommandWithValue>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success("OK"));

        var factory = new Mock<IRequestContextFactory>();

        var executor = new PipelineExecutor(resolver.Object);

        // Act
        var result = await executor.ExecuteAsync(new TestCommandWithValue("x"), handler.Object, factory.Object, CancellationToken.None);

        // Assert — handler ran and the context was never created (no active behavior needed it).
        result.Value.Should().Be("OK");
        handler.Verify(h => h.Handle(It.IsAny<TestCommandWithValue>(), It.IsAny<CancellationToken>()), Times.Once);
        factory.Verify(f => f.Create<TestCommandWithValue, Result<string>>(It.IsAny<TestCommandWithValue>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_LazyContext_Creates_Context_When_A_Behavior_Is_Active()
    {
        // Arrange — an active behavior, so the executor must create the context for it.
        var log = new List<string>();
        var resolver = new Mock<IHandlerResolver>();
        resolver
            .Setup(r => r.ResolveAll<IPipelineBehavior<TestCommandWithValue, Result<string>>>())
            .Returns(new IPipelineBehavior<TestCommandWithValue, Result<string>>[] { new OrderedBehavior(1, "Active", log) });

        var request = new TestCommandWithValue("x");
        var context = new RequestContext<TestCommandWithValue, Result<string>>(request, "user");
        var factory = new Mock<IRequestContextFactory>();
        factory.Setup(f => f.Create<TestCommandWithValue, Result<string>>(request, It.IsAny<CancellationToken>())).Returns(context);

        var handler = new Mock<Mediarq.Core.Common.Requests.Abstraction.IRequestHandler<TestCommandWithValue, Result<string>>>();
        handler.Setup(h => h.Handle(request, It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success("OK"));

        var executor = new PipelineExecutor(resolver.Object);

        // Act
        var result = await executor.ExecuteAsync(request, handler.Object, factory.Object, CancellationToken.None);

        // Assert
        result.Value.Should().Be("OK");
        log.Should().ContainInOrder("Before Active", "After Active");
        factory.Verify(f => f.Create<TestCommandWithValue, Result<string>>(request, It.IsAny<CancellationToken>()), Times.Once);
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

    private sealed class ConditionalBehavior(bool isActive, string name, List<string> log)
        : IPipelineBehavior<TestCommandWithValue, Result<string>>, IConditionalPipelineBehavior
    {
        public bool IsActive { get; } = isActive;

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