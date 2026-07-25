using System.Threading.Channels;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Deferred;

/// <inheritdoc />
internal sealed class ChannelDeferredDispatcher(Channel<Func<IServiceProvider, CancellationToken, Task>> channel) : IDeferredDispatcher
{
    /// <inheritdoc />
    public ValueTask SendLaterAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);

        return channel.Writer.WriteAsync(
            (sp, ct) => sp.GetRequiredService<ISender>().Send(command, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask PublishLaterAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        return channel.Writer.WriteAsync(
            (sp, ct) => sp.GetRequiredService<IPublisher>().Publish(notification, ct),
            cancellationToken);
    }
}
