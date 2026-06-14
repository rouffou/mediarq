using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Exceptions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Results;

namespace Mediarq.Tests.Data;

/// <summary>Scoped sink used by the integration fixtures to record pipeline/handler execution.</summary>
public sealed class ExecutionTrace
{
    public List<string> Entries { get; } = [];
}

// --- Command returning a value ---
public record PingCommand(string Text) : ICommand<Result<string>>;

public sealed class PingCommandHandler : ICommandHandler<PingCommand, Result<string>>
{
    public Task<Result<string>> Handle(PingCommand request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Text));
}

// --- No-result (void) command, routed through the pipeline as a Unit request ---
public record TracedVoidCommand : ICommand;

public sealed class TracedVoidCommandHandler(ExecutionTrace trace) : ICommandHandler<TracedVoidCommand>
{
    public Task Handle(TracedVoidCommand request, CancellationToken cancellationToken = default)
    {
        trace.Entries.Add("handler");
        return Task.CompletedTask;
    }
}

// --- Command guarded by a validator ---
public record ValidatedCommand(string Name) : ICommand<Result<string>>;

public sealed class ValidatedCommandHandler : ICommandHandler<ValidatedCommand, Result<string>>
{
    public Task<Result<string>> Handle(ValidatedCommand request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Name));
}

public sealed class ValidatedCommandValidator : IValidator<ValidatedCommand>
{
    public IEnumerable<ValidationResult> Validate(ValidatedCommand instance)
    {
        if (string.IsNullOrWhiteSpace(instance.Name))
        {
            yield return ValidationResult.Failure([new ValidationPropertyError(nameof(instance.Name), "Name is required.")]);
        }
        else
        {
            yield return ValidationResult.Success();
        }
    }
}

// --- Pipeline behavior recording execution into the scoped trace ---
public sealed class TraceBehavior<TRequest, TResponse>(ExecutionTrace trace) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        trace.Entries.Add($"before:{typeof(TRequest).Name}");
        TResponse response = await handle();
        trace.Entries.Add($"after:{typeof(TRequest).Name}");
        return response;
    }
}

// --- Notification handled by multiple handlers ---
public record OrderPlaced(int OrderId) : INotification;

public sealed class OrderPlacedAuditHandler(ExecutionTrace trace) : INotificationHandler<OrderPlaced>
{
    public Task Handle(OrderPlaced notification, CancellationToken cancellationToken = default)
    {
        trace.Entries.Add($"audit:{notification.OrderId}");
        return Task.CompletedTask;
    }
}

public sealed class OrderPlacedEmailHandler(ExecutionTrace trace) : INotificationHandler<OrderPlaced>
{
    public Task Handle(OrderPlaced notification, CancellationToken cancellationToken = default)
    {
        trace.Entries.Add($"email:{notification.OrderId}");
        return Task.CompletedTask;
    }
}

// --- Command whose handler throws, with an exception handler turning it into a failed Result ---
public record ThrowingCommand(string Message) : ICommand<Result<string>>;

public sealed class ThrowingCommandHandler : ICommandHandler<ThrowingCommand, Result<string>>
{
    public Task<Result<string>> Handle(ThrowingCommand request, CancellationToken cancellationToken = default)
        => throw new InvalidOperationException(request.Message);
}

public sealed class ThrowingCommandExceptionHandler : IRequestExceptionHandler<ThrowingCommand, Result<string>>
{
    public Task Handle(ThrowingCommand request, Exception exception, RequestExceptionHandlerState<Result<string>> state, CancellationToken cancellationToken = default)
    {
        state.SetHandled(Result.Failure<string>(ResultError.Failure("Command.Failed", exception.Message)));
        return Task.CompletedTask;
    }
}
