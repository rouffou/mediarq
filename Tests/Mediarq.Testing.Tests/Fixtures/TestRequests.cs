using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Common.Results;

namespace Mediarq.Testing.Tests.Fixtures;

public sealed record CountStream(int Count) : IStreamRequest<int>;

public sealed record PingCommand(string Message) : ICommand<Result<string>>;

public sealed class PingCommandHandler : ICommandHandler<PingCommand, Result<string>>
{
    public Task<Result<string>> Handle(PingCommand request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Message));
}

public sealed record VoidCommand : ICommand;

public sealed class VoidCommandHandler : ICommandHandler<VoidCommand>
{
    public Task Handle(VoidCommand request, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed record Pinged(int Id) : INotification;

public sealed class PingedHandler(List<int> handledIds) : INotificationHandler<Pinged>
{
    public Task Handle(Pinged notification, CancellationToken cancellationToken = default)
    {
        handledIds.Add(notification.Id);
        return Task.CompletedTask;
    }
}
