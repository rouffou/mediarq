using System.Text.Json;
using Mediarq.Core.Common.Requests.Command;
using Quartz;

namespace Mediarq.Quartz;

/// <summary>
/// Enqueue or schedule a Mediarq command as a Quartz.NET job. The command is JSON-serialized (via
/// <see cref="JsonSerializer"/>) into the job's <see cref="JobDataMap"/> alongside its
/// <see cref="Type.AssemblyQualifiedName"/>, and reconstructed by <see cref="MediarqQuartzJob"/> when the
/// trigger fires.
/// </summary>
/// <remarks>
/// Requires Quartz's dependency-injection integration (<c>services.AddQuartz()</c>, from
/// <c>Quartz.Extensions.DependencyInjection</c> — its DI job factory is the default, no extra
/// configuration needed) so <c>MediarqQuartzJob</c> — and everything the command's handler depends on —
/// resolves correctly when the job runs. See the package README.
/// </remarks>
public static class QuartzSchedulingExtensions
{
    /// <summary>Schedules <paramref name="command"/> to run as soon as the scheduler picks up the trigger.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="scheduler">The Quartz scheduler to schedule the job on.</param>
    /// <param name="command">The command to dispatch when the job fires.</param>
    /// <param name="cancellationToken">A token to cancel the scheduling call.</param>
    /// <returns>The trigger's next fire time.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> or <paramref name="command"/> is <see langword="null"/>.</exception>
    public static Task<DateTimeOffset> EnqueueAsync<TCommand>(this IScheduler scheduler, TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
        => ScheduleAsync(scheduler, command, TriggerBuilder.Create().StartNow(), cancellationToken);

    /// <summary>Schedules <paramref name="command"/> to run after <paramref name="delay"/>.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="scheduler">The Quartz scheduler to schedule the job on.</param>
    /// <param name="command">The command to dispatch when the job fires.</param>
    /// <param name="delay">How long to wait before the job becomes eligible to run.</param>
    /// <param name="cancellationToken">A token to cancel the scheduling call.</param>
    /// <returns>The trigger's next fire time.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> or <paramref name="command"/> is <see langword="null"/>.</exception>
    public static Task<DateTimeOffset> ScheduleAsync<TCommand>(this IScheduler scheduler, TCommand command, TimeSpan delay, CancellationToken cancellationToken = default)
        where TCommand : ICommand
        => ScheduleAsync(scheduler, command, TriggerBuilder.Create().StartAt(DateTimeOffset.UtcNow.Add(delay)), cancellationToken);

    /// <summary>Schedules <paramref name="command"/> to run at <paramref name="startAt"/>.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="scheduler">The Quartz scheduler to schedule the job on.</param>
    /// <param name="command">The command to dispatch when the job fires.</param>
    /// <param name="startAt">The point in time the job becomes eligible to run.</param>
    /// <param name="cancellationToken">A token to cancel the scheduling call.</param>
    /// <returns>The trigger's next fire time.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheduler"/> or <paramref name="command"/> is <see langword="null"/>.</exception>
    public static Task<DateTimeOffset> ScheduleAsync<TCommand>(this IScheduler scheduler, TCommand command, DateTimeOffset startAt, CancellationToken cancellationToken = default)
        where TCommand : ICommand
        => ScheduleAsync(scheduler, command, TriggerBuilder.Create().StartAt(startAt), cancellationToken);

    private static Task<DateTimeOffset> ScheduleAsync<TCommand>(IScheduler scheduler, TCommand command, TriggerBuilder triggerBuilder, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(scheduler);
        ArgumentNullException.ThrowIfNull(command);

        var commandType = typeof(TCommand);
        var job = JobBuilder.Create<MediarqQuartzJob>()
            .WithIdentity(Guid.NewGuid().ToString("N"))
            .UsingJobData(MediarqQuartzJob.CommandTypeKey, commandType.AssemblyQualifiedName)
            .UsingJobData(MediarqQuartzJob.CommandPayloadKey, JsonSerializer.Serialize(command, commandType))
            .Build();

        return scheduler.ScheduleJob(job, triggerBuilder.Build(), cancellationToken);
    }
}
