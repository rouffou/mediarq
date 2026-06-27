using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.Extensions.Logging;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>Raised after an order is created; fanned out to every registered handler.</summary>
public sealed record OrderPlaced(Guid OrderId, string Customer, decimal Total) : INotification;

/// <summary>One of several handlers: writes an audit log line.</summary>
public sealed class LogOrderPlacedHandler(ILogger<LogOrderPlacedHandler> logger)
    : INotificationHandler<OrderPlaced>
{
    public Task Handle(OrderPlaced notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Order {OrderId} placed by {Customer} for {Total:0.00}",
            notification.OrderId, notification.Customer, notification.Total);
        return Task.CompletedTask;
    }
}

/// <summary>Another handler for the same event: simulates queuing a confirmation email.</summary>
public sealed class SendOrderConfirmationHandler(ILogger<SendOrderConfirmationHandler> logger)
    : INotificationHandler<OrderPlaced>
{
    public Task Handle(OrderPlaced notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Confirmation email queued for {Customer}", notification.Customer);
        return Task.CompletedTask;
    }
}
