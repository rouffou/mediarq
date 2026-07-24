using Hangfire;
using Mediarq.Core.Common.Requests.Command;

namespace Mediarq.Hangfire;

/// <summary>
/// Enqueue or schedule a Mediarq command as a Hangfire background job. Each method captures the command's
/// own compile-time closed type (rather than the <see cref="ICommand"/> interface) so Hangfire's job
/// serializer can round-trip it correctly — passing a variable statically typed as <see cref="ICommand"/>
/// itself would make Hangfire store the interface as the parameter type and fail to deserialize the
/// concrete command back.
/// </summary>
/// <remarks>
/// Requires a running Hangfire server (<c>AddHangfireServer()</c>) wired with an activator that resolves
/// scoped services (e.g. <c>Hangfire.AspNetCore</c>'s <c>AspNetCoreJobActivator</c>, used automatically by
/// <c>services.AddHangfire(...)</c> in an ASP.NET Core app) so <see cref="IMediarqJobDispatcher"/> — and
/// everything the command's handler depends on — resolves correctly when the job runs.
/// </remarks>
public static class HangfireSchedulingExtensions
{
    /// <summary>Enqueues <paramref name="command"/> to run as soon as a worker is available.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="client">The Hangfire client to enqueue the job on.</param>
    /// <param name="command">The command to dispatch when the job runs.</param>
    /// <returns>The Hangfire job id.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> or <paramref name="command"/> is <see langword="null"/>.</exception>
    public static string Enqueue<TCommand>(this IBackgroundJobClient client, TCommand command)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(command);

        return client.Enqueue<IMediarqJobDispatcher>(d => d.DispatchAsync(command));
    }

    /// <summary>Schedules <paramref name="command"/> to run after <paramref name="delay"/>.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="client">The Hangfire client to schedule the job on.</param>
    /// <param name="command">The command to dispatch when the job runs.</param>
    /// <param name="delay">How long to wait before the job becomes eligible to run.</param>
    /// <returns>The Hangfire job id.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> or <paramref name="command"/> is <see langword="null"/>.</exception>
    public static string Schedule<TCommand>(this IBackgroundJobClient client, TCommand command, TimeSpan delay)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(command);

        return client.Schedule<IMediarqJobDispatcher>(d => d.DispatchAsync(command), delay);
    }

    /// <summary>Schedules <paramref name="command"/> to run at <paramref name="enqueueAt"/>.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="client">The Hangfire client to schedule the job on.</param>
    /// <param name="command">The command to dispatch when the job runs.</param>
    /// <param name="enqueueAt">The point in time the job becomes eligible to run.</param>
    /// <returns>The Hangfire job id.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> or <paramref name="command"/> is <see langword="null"/>.</exception>
    public static string Schedule<TCommand>(this IBackgroundJobClient client, TCommand command, DateTimeOffset enqueueAt)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(command);

        return client.Schedule<IMediarqJobDispatcher>(d => d.DispatchAsync(command), enqueueAt);
    }
}
