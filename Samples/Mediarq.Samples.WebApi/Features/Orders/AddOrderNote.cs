using System.ComponentModel.DataAnnotations;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Attaches a note to an order. Validated with <c>System.ComponentModel.DataAnnotations</c> attributes
/// (the alternative to FluentValidation), bridged into the pipeline by AddMediarqDataAnnotations().
/// </summary>
public sealed record AddOrderNoteCommand : ICommand<Result>
{
    [Required]
    public Guid OrderId { get; init; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(200, MinimumLength = 1)]
    public string Note { get; init; } = string.Empty;
}

public sealed class AddOrderNoteHandler(AppDbContext db)
    : ICommandHandler<AddOrderNoteCommand, Result>
{
    public async Task<Result> Handle(AddOrderNoteCommand request, CancellationToken cancellationToken = default)
    {
        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure(ResultError.NotFound("Order.NotFound", $"Order {request.OrderId} was not found."));

        order.Note = request.Note;
        await db.SaveChangesAsync(cancellationToken);

        // Cascaded notification: attached to the successful Result instead of injecting IPublisher and
        // calling Publish(...) explicitly. Published automatically after dispatch, and only on success —
        // a lightweight, in-process alternative to the transactional outbox (CreateOrder) and domain
        // events (ConfirmOrder) used elsewhere in this sample.
        return Result.Success().WithNotifications(new OrderNoteAddedEvent(order.Id, request.Note));
    }
}

/// <summary>Cascaded via <see cref="Result.WithNotifications"/> — no explicit <c>IPublisher</c> injection needed.</summary>
public sealed record OrderNoteAddedEvent(Guid OrderId, string Note) : INotification;

public sealed class LogOrderNoteAddedHandler(ILogger<LogOrderNoteAddedHandler> logger)
    : INotificationHandler<OrderNoteAddedEvent>
{
    public Task Handle(OrderNoteAddedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[cascaded notification] note added to order {OrderId}: {Note}",
            notification.OrderId, notification.Note);
        return Task.CompletedTask;
    }
}
