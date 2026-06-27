using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Exceptions;
using Mediarq.Core.Common.Results;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>A command whose handler may throw (e.g. a payment gateway error).</summary>
public sealed record ChargePaymentCommand(Guid OrderId, bool SimulateGatewayError) : ICommand<Result<string>>;

public sealed class ChargePaymentCommandHandler : ICommandHandler<ChargePaymentCommand, Result<string>>
{
    public Task<Result<string>> Handle(ChargePaymentCommand request, CancellationToken cancellationToken = default)
    {
        if (request.SimulateGatewayError)
            throw new InvalidOperationException("Payment gateway timed out.");

        return Task.FromResult(Result.Success($"charged order {request.OrderId}"));
    }
}

/// <summary>
/// Turns the thrown exception into a failed <see cref="Result{T}"/> instead of letting it propagate —
/// the railway-oriented way. The RequestExceptionProcessorBehavior invokes this when the handler throws.
/// </summary>
public sealed class ChargePaymentExceptionHandler
    : IRequestExceptionHandler<ChargePaymentCommand, Result<string>>
{
    public Task Handle(
        ChargePaymentCommand request,
        Exception exception,
        RequestExceptionHandlerState<Result<string>> state,
        CancellationToken cancellationToken = default)
    {
        state.SetHandled(Result.Failure<string>(
            ResultError.Problem("Payment.Failed", exception.Message)));
        return Task.CompletedTask;
    }
}
