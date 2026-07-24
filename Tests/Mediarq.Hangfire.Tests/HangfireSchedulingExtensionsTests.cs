using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Mediarq.Core.Common.Requests.Command;

namespace Mediarq.Hangfire.Tests;

public class HangfireSchedulingExtensionsTests
{
    private sealed record TestCommand(string Value) : ICommand;

    private sealed class RecordingBackgroundJobClient : IBackgroundJobClient
    {
        public Job? LastJob { get; private set; }
        public IState? LastState { get; private set; }

        public string Create(Job job, IState state)
        {
            LastJob = job;
            LastState = state;
            return "job-id";
        }

        public bool ChangeState(string jobId, IState state, string? expectedState) => true;
    }

    [Fact]
    public void Enqueue_Creates_A_Job_Targeting_DispatchAsync_With_The_Command()
    {
        var client = new RecordingBackgroundJobClient();
        var command = new TestCommand("x");

        var id = client.Enqueue(command);

        id.Should().Be("job-id");
        client.LastJob.Should().NotBeNull();
        client.LastJob!.Type.Should().Be(typeof(IMediarqJobDispatcher));
        client.LastJob.Method.Name.Should().Be(nameof(IMediarqJobDispatcher.DispatchAsync));
        client.LastJob.Args.Should().ContainSingle().Which.Should().Be(command);
        client.LastState.Should().BeOfType<EnqueuedState>();
    }

    [Fact]
    public void Schedule_With_A_Delay_Creates_A_Scheduled_Job()
    {
        var client = new RecordingBackgroundJobClient();
        var command = new TestCommand("x");

        client.Schedule(command, TimeSpan.FromMinutes(5));

        client.LastJob.Should().NotBeNull();
        client.LastJob!.Args.Should().ContainSingle().Which.Should().Be(command);
        client.LastState.Should().BeOfType<ScheduledState>();
    }

    [Fact]
    public void Schedule_With_A_Specific_Time_Creates_A_Scheduled_Job()
    {
        var client = new RecordingBackgroundJobClient();
        var command = new TestCommand("x");
        var enqueueAt = DateTimeOffset.UtcNow.AddHours(1);

        client.Schedule(command, enqueueAt);

        client.LastJob.Should().NotBeNull();
        client.LastState.Should().BeOfType<ScheduledState>();
    }

    [Fact]
    public void Enqueue_Throws_For_A_Null_Client()
    {
        IBackgroundJobClient client = null!;

        var act = () => client.Enqueue(new TestCommand("x"));

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Enqueue_Throws_For_A_Null_Command()
    {
        var client = new RecordingBackgroundJobClient();

        var act = () => client.Enqueue<TestCommand>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Schedule_Throws_For_A_Null_Command()
    {
        var client = new RecordingBackgroundJobClient();

        var act = () => client.Schedule<TestCommand>(null!, TimeSpan.FromMinutes(1));

        act.Should().Throw<ArgumentNullException>();
    }
}
