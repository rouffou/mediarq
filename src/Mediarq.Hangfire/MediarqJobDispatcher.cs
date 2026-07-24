using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Mediators;

namespace Mediarq.Hangfire;

/// <inheritdoc />
internal sealed class MediarqJobDispatcher(ISender sender) : IMediarqJobDispatcher
{
    /// <inheritdoc />
    public Task DispatchAsync<TCommand>(TCommand command)
        where TCommand : ICommand
        => sender.Send(command, CancellationToken.None);
}
