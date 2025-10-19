namespace Mediarq.Tests.Core.Common.Pipeline.Behaviors
{
    using FluentAssertions;
    using Mediarq.Core.Common.Contexts;
    using Mediarq.Core.Common.Pipeline.Behaviors;
    using Mediarq.Core.Common.Results;
    using Mediarq.Core.Common.Time;
    using Mediarq.Tests.Data;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class PerformanceBehaviorTests
    {
        private readonly Mock<ILogger<PerformanceBehavior<TestCommand, Result>>> _mockLogger;
        private readonly Mock<IClock> _mockClock;

        public PerformanceBehaviorTests()
        {
            _mockLogger = new Mock<ILogger<PerformanceBehavior<TestCommand, Result>>>();
            _mockClock = new Mock<IClock>();
        }


        [Fact]
        public async Task Handle_Should_InvokeNext()
        {
            // Arrange
            var behavior = new PerformanceBehavior<TestCommand, Result>(_mockLogger.Object, _mockClock.Object);
            var context = new Mock<IMutableRequestContext<TestCommand, Result>>().Object;
            var wasCalled = false;

            Task<Result> Next() { wasCalled = true; return Task.FromResult(Result.Success()); }

            // Act
            var result = await behavior.Handle(context, Next, CancellationToken.None);

            // Assert
            wasCalled.Should().BeTrue();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_Should_NotLogWarning_WhenUnderThreshold()
        {
            // Arrange
            var behavior = new PerformanceBehavior<TestCommand, Result>(_mockLogger.Object, _mockClock.Object);
            var context = new Mock<IMutableRequestContext<TestCommand, Result>>().Object;

            Task<Result> Next() => Task.FromResult(Result.Success());

            // Act
            await behavior.Handle(context, Next, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_Should_LogWarning_WhenOverThreshold()
        {
            // Arrange
            var behavior = new PerformanceBehavior<TestCommand, Result>(_mockLogger.Object, _mockClock.Object);
            var context = new Mock<IMutableRequestContext<TestCommand, Result>>().Object;

            Task<Result> Next()
            {
                // Simuler une opération longue
                Thread.Sleep(600);
                return Task.FromResult(Result.Success());
            }

            // Act
            await behavior.Handle(context, Next, CancellationToken.None);

            // Assert
            _mockLogger.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Long running request")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_WhenRequestIsNull()
        {
            // Arrange
            var behavior = new PerformanceBehavior<TestCommand, Result>(_mockLogger.Object, _mockClock.Object);

            // Act & Assert
            await FluentActions
                .Invoking(() => behavior.Handle(null!, () => Task.FromResult(Result.Success()), CancellationToken.None))
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithParameterName("request");
        }

        [Fact]
        public async Task Handle_Should_ThrowArgumentNullException_WhenNextIsNull()
        {
            // Arrange
            var behavior = new PerformanceBehavior<TestCommand, Result>(_mockLogger.Object, _mockClock.Object);
            var context = new Mock<IMutableRequestContext<TestCommand, Result>>().Object;

            // Act & Assert
            await FluentActions
                .Invoking(() => behavior.Handle(context, null!, CancellationToken.None))
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithParameterName("next");
        }
    }
}