using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Moq;

namespace Mediarq.Tests.Core.Common.Pipeline.Behaviors;

public class TimeoutBehaviorTests
{
    public record SlowCommand(TimeSpan Timeout) : ICommand<Result<string>>, ITimeoutRequest;

    public record PlainCommand : ICommand<Result<string>>;

    private static IMutableRequestContext<SlowCommand, Result<string>> ContextFor(SlowCommand command)
    {
        var context = new Mock<IMutableRequestContext<SlowCommand, Result<string>>>();
        context.SetupGet(c => c.Request).Returns(command);
        return context.Object;
    }

    [Fact]
    public void IsActive_True_For_TimeoutRequest_False_Otherwise()
    {
        new TimeoutBehavior<SlowCommand, Result<string>>().IsActive.Should().BeTrue();
        new TimeoutBehavior<PlainCommand, Result<string>>().IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Returns_Result_When_Handler_Completes_Before_Timeout()
    {
        var behavior = new TimeoutBehavior<SlowCommand, Result<string>>();
        var context = ContextFor(new SlowCommand(TimeSpan.FromSeconds(5)));

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")), CancellationToken.None);

        result.Value.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_Throws_RequestTimeoutException_When_Handler_Exceeds_Timeout()
    {
        var behavior = new TimeoutBehavior<SlowCommand, Result<string>>();
        var timeout = TimeSpan.FromMilliseconds(50);
        var context = ContextFor(new SlowCommand(timeout));

        Func<Task<Result<string>>> slowHandler = async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5), CancellationToken.None);
            return Result.Success("late");
        };

        var act = () => behavior.Handle(context, slowHandler, CancellationToken.None);

        (await act.Should().ThrowAsync<RequestTimeoutException>())
            .Which.Timeout.Should().Be(timeout);
    }

    [Fact]
    public async Task Handle_Skips_Timeout_When_Timeout_Is_Not_Positive()
    {
        var behavior = new TimeoutBehavior<SlowCommand, Result<string>>();
        var context = ContextFor(new SlowCommand(TimeSpan.Zero));

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")), CancellationToken.None);

        result.Value.Should().Be("ok");
    }
}
