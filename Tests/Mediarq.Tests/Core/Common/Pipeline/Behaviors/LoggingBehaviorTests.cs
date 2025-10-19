namespace Mediarq.Tests.Core.Common.Pipeline.Behaviors;

using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Results;
using Mediarq.Tests.Data;
using Mediarq.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class LoggingBehaviorTests
{
    private readonly Mock<ILogger<LoggingBehavior<TestCommand, Result>>> _mockLogger;
    private readonly Mock<IMutableRequestContext<TestCommand, Result>> _mockContext;

    public LoggingBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<LoggingBehavior<TestCommand, Result>>>();
        _mockContext = new Mock<IMutableRequestContext<TestCommand, Result>>();

        _mockContext.SetupGet(c => c.RequestId).Returns(Guid.NewGuid());
        _mockContext.SetupGet(c => c.StartedAt).Returns(DateTime.UtcNow);
        _mockContext.SetupGet(c => c.FinishedAt).Returns(DateTime.UtcNow.AddSeconds(1));
        _mockContext.SetupGet(c => c.Request).Returns(new TestCommand("UnitTest"));
    }

    [Fact]
    public async Task Handle_Should_Log_Entry_And_Exit()
    {
        // Arrange
        var behavior = new LoggingBehavior<TestCommand, Result>(_mockLogger.Object);

        // Act
        var result = await behavior.Handle(_mockContext.Object, () => Task.FromResult(Result.Success()));

        // Assert
        result.IsSuccess.Should().BeTrue();

        _mockLogger.VerifyLog(LogLevel.Information, Times.Exactly(2), msg =>
            msg.Contains("Handling") || msg.Contains("Handled"));
    }

    [Fact]
    public async Task Handle_Should_Invoke_Next_Delegate()
    {
        // Arrange
        var behavior = new LoggingBehavior<TestCommand, Result>(_mockLogger.Object);
        var called = false;

        // Act
        var result = await behavior.Handle(_mockContext.Object, () =>
        {
            called = true;
            return Task.FromResult(Result.Success());
        });

        // Assert
        called.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_Log_Request_Type_And_RequestId()
    {
        // Arrange
        var behavior = new LoggingBehavior<TestCommand, Result>(_mockLogger.Object);
        var requestId = Guid.NewGuid();

        _mockContext.SetupGet(c => c.RequestId).Returns(requestId);

        // Act
        await behavior.Handle(_mockContext.Object, () => Task.FromResult(Result.Success()));

        // Assert
        _mockLogger.VerifyLog(LogLevel.Information, Times.AtLeastOnce(), message =>
            message.Contains(nameof(TestCommand)) &&
            message.Contains(requestId.ToString()));
    }

    [Fact]
    public async Task CannotCallHandleWithNullRequest()
    {
        // Arrange
        var behavior = new LoggingBehavior<TestCommand, Result>(_mockLogger.Object);
        Func<Task<Result>> next = () => Task.FromResult(Result.Success());

        // Act & Assert
        await FluentActions
            .Invoking(() => behavior.Handle(null!, next, CancellationToken.None))
            .Should()
            .ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task CannotCallHandleWithNullNext()
    {
        var behavior = new LoggingBehavior<TestCommand, Result>(_mockLogger.Object);
        await FluentActions
                .Invoking(() => behavior.Handle(new Mock<IMutableRequestContext<TestCommand, Result>>().Object, default, CancellationToken.None))
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithParameterName("next");
    }
}