using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Deferred;

/// <summary>
/// Queues a command or notification for asynchronous, in-process dispatch on a background worker instead
/// of running its handler(s) inline — decoupling the caller from handler execution time. Backed by a
/// <see cref="System.Threading.Channels.Channel{T}"/>, not a persistent store: queued work does not
/// survive a process crash, and <c>Mediarq.Hangfire</c>/<c>Mediarq.Quartz</c> remain the right choice for
/// delayed, cron, or durable scheduling. See <see cref="DeferredDispatchHostedService"/> for the
/// drain-on-shutdown guarantee this gives for a graceful stop.
/// </summary>
public interface IDeferredDispatcher
{
    /// <summary>Queues <paramref name="command"/> to be sent through the real pipeline by the background worker.</summary>
    /// <typeparam name="TCommand">The concrete command type.</typeparam>
    /// <param name="command">The command to dispatch.</param>
    /// <param name="cancellationToken">Cancels only the act of enqueuing, not the eventual dispatch.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="command"/> is <see langword="null"/>.</exception>
    ValueTask SendLaterAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>Queues <paramref name="notification"/> to be published through the real pipeline by the background worker.</summary>
    /// <typeparam name="TNotification">The concrete notification type.</typeparam>
    /// <param name="notification">The notification to publish.</param>
    /// <param name="cancellationToken">Cancels only the act of enqueuing, not the eventual dispatch.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="notification"/> is <see langword="null"/>.</exception>
    ValueTask PublishLaterAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
