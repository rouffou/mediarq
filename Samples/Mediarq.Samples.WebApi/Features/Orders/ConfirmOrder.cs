using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Idempotency;
using Mediarq.Samples.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Confirms an order. Implements <see cref="IIdempotentRequest"/>: the IdempotencyBehavior runs it at
/// most once per <see cref="IdempotencyKey"/> (taken from the <c>Idempotency-Key</c> header) and replays
/// the stored result for repeated calls — safe to retry a POST without double-confirming.
/// </summary>
public sealed record ConfirmOrderCommand(Guid OrderId, string IdempotencyKey)
    : ICommand<Result>, IIdempotentRequest
{
    public TimeSpan? IdempotencyDuration => TimeSpan.FromMinutes(10);
}

public sealed class ConfirmOrderHandler(AppDbContext db, ILogger<ConfirmOrderHandler> logger)
    : ICommandHandler<ConfirmOrderCommand, Result>
{
    public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(ResultError.NotFound("Order.NotFound", $"Order {request.OrderId} was not found."));

        // Strip CR/LF from the user-supplied key before logging to prevent log forging (CWE-117).
        var sanitizedKey = request.IdempotencyKey.Replace("\r", "").Replace("\n", "");
        // Only logged on the first call for a given key; replays skip the handler entirely.
        logger.LogInformation("Confirming order {OrderId} (key {Key})", request.OrderId, sanitizedKey);

        order.Status = OrderStatus.Confirmed;
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
