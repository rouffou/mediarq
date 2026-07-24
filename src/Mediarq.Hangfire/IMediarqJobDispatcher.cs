using Mediarq.Core.Common.Requests.Command;

namespace Mediarq.Hangfire;

/// <summary>
/// The job method Hangfire actually invokes when a command scheduled via
/// <see cref="HangfireSchedulingExtensions"/> runs. Not meant to be called directly — go through
/// <see cref="HangfireSchedulingExtensions"/> instead, which builds the correctly-typed job expression
/// Hangfire needs to serialize and later reconstruct the command.
/// </summary>
public interface IMediarqJobDispatcher
{
    /// <summary>Dispatches <paramref name="command"/> through the real Mediarq pipeline.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync<TCommand>(TCommand command)
        where TCommand : ICommand;
}
