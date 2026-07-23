namespace Mediarq.MediatRCompat.Tests.Data;

// A minimal MediatR-shaped request/handler/notification/stream surface, unchanged from what a real
// MediatR-based application would have, used to prove Mediarq.MediatRCompat dispatches them through
// Mediarq's own pipeline without requiring any code change to these classes.

public sealed class ExecutionTrace
{
    public List<string> Entries { get; } = [];
}

public sealed record Ping(string Message) : MediatR.IRequest<string>;

public sealed class PingHandler : MediatR.IRequestHandler<Ping, string>
{
    public Task<string> Handle(Ping request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public sealed record TracedVoidCommand : MediatR.IRequest;

public sealed class TracedVoidCommandHandler(ExecutionTrace trace) : MediatR.IRequestHandler<TracedVoidCommand>
{
    public Task Handle(TracedVoidCommand request, CancellationToken cancellationToken)
    {
        trace.Entries.Add("handler");
        return Task.CompletedTask;
    }
}

public sealed record OrderPlaced(int OrderId) : MediatR.INotification;

public sealed class AuditOrderPlacedHandler(ExecutionTrace trace) : MediatR.INotificationHandler<OrderPlaced>
{
    public Task Handle(OrderPlaced notification, CancellationToken cancellationToken)
    {
        trace.Entries.Add($"audit:{notification.OrderId}");
        return Task.CompletedTask;
    }
}

public sealed class EmailOrderPlacedHandler(ExecutionTrace trace) : MediatR.INotificationHandler<OrderPlaced>
{
    public Task Handle(OrderPlaced notification, CancellationToken cancellationToken)
    {
        trace.Entries.Add($"email:{notification.OrderId}");
        return Task.CompletedTask;
    }
}

public sealed record CountStream(int Count) : MediatR.IStreamRequest<int>;

public sealed class CountStreamHandler : MediatR.IStreamRequestHandler<CountStream, int>
{
    public async IAsyncEnumerable<int> Handle(CountStream request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 1; i <= request.Count; i++)
        {
            await Task.Yield();
            yield return i;
        }
    }
}
