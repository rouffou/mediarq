using Mediarq.Authorization;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Results;
using Mediarq.Idempotency;
using Mediarq.Samples.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Confirms an order. Implements <see cref="IIdempotentRequest"/>: the IdempotencyBehavior runs it at
/// most once per <see cref="IdempotencyKey"/> (taken from the <c>Idempotency-Key</c> header) and replays
/// the stored result for repeated calls — safe to retry a POST without double-confirming. Also implements
/// <see cref="IAuthorizedRequest"/>: only a caller satisfying the "orders:confirm" policy (see
/// <c>Program.cs</c>'s demo header-based auth scheme) may confirm an order.
/// </summary>
public sealed record ConfirmOrderCommand(Guid OrderId, string IdempotencyKey)
    : ICommand<Result>, IIdempotentRequest, IAuthorizedRequest
{
    public TimeSpan? IdempotencyDuration => TimeSpan.FromMinutes(10);
    public string? PolicyName => "orders:confirm";
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
        // Only logged on the first call for a given key; replays skip the handler entirely.
        var sanitizedKey = request.IdempotencyKey
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
        logger.LogInformation("Confirming order {OrderId} (key {Key})", request.OrderId, sanitizedKey);

        order.Confirm();
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

/// <summary>
/// Raised by <see cref="Order.Confirm"/> and published by <c>DomainEventsInterceptor</c> right after
/// <c>SaveChanges</c> commits — an in-process side effect, unlike <see cref="OrderPlacedEvent"/> which
/// also crosses the bus via the transactional outbox.
/// </summary>
public sealed record OrderConfirmedDomainEvent(Guid OrderId, string Customer) : INotification;

public sealed class LogOrderConfirmedHandler(ILogger<LogOrderConfirmedHandler> logger)
    : INotificationHandler<OrderConfirmedDomainEvent>
{
    public Task Handle(OrderConfirmedDomainEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[domain event] order {OrderId} confirmed for {Customer}",
            notification.OrderId, notification.Customer);
        return Task.CompletedTask;
    }
}
