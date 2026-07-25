using FluentAssertions;
using Mediarq.Core.Common.Requests.Command;
using Quartz;
using Quartz.Impl;

namespace Mediarq.Quartz.Tests;

public class QuartzSchedulingExtensionsTests
{
    private sealed record TestCommand(string Value) : ICommand;

    private static async Task<IScheduler> NewSchedulerAsync()
        => await new StdSchedulerFactory().GetScheduler();

    [Fact]
    public async Task EnqueueAsync_Throws_For_A_Null_Scheduler()
    {
        IScheduler scheduler = null!;

        var act = () => scheduler.EnqueueAsync(new TestCommand("x"));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task EnqueueAsync_Throws_For_A_Null_Command()
    {
        var scheduler = await NewSchedulerAsync();

        var act = () => scheduler.EnqueueAsync<TestCommand>(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ScheduleAsync_With_A_Delay_Throws_For_A_Null_Command()
    {
        var scheduler = await NewSchedulerAsync();

        var act = () => scheduler.ScheduleAsync<TestCommand>(null!, TimeSpan.FromMinutes(1));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ScheduleAsync_With_A_Specific_Time_Throws_For_A_Null_Command()
    {
        var scheduler = await NewSchedulerAsync();

        var act = () => scheduler.ScheduleAsync<TestCommand>(null!, DateTimeOffset.UtcNow.AddHours(1));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task EnqueueAsync_Schedules_A_Job_Carrying_The_Serialized_Command()
    {
        var scheduler = await NewSchedulerAsync();
        var command = new TestCommand("hello");

        await scheduler.EnqueueAsync(command);

        var jobKeys = await scheduler.GetJobKeys(global::Quartz.Impl.Matchers.GroupMatcher<JobKey>.AnyGroup());
        jobKeys.Should().ContainSingle();

        var jobDetail = await scheduler.GetJobDetail(jobKeys.Single());
        jobDetail.Should().NotBeNull();
        jobDetail!.JobDataMap.GetString(MediarqQuartzJob.CommandTypeKey).Should().Contain(nameof(TestCommand));
        jobDetail.JobDataMap.GetString(MediarqQuartzJob.CommandPayloadKey).Should().Contain("hello");
    }
}
