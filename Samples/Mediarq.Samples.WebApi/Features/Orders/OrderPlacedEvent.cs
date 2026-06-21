using MassTransit;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.MassTransit;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Raised once an order is committed (published from the outbox). It is an <see cref="IIntegrationEvent"/>,
/// so AddMediarqMassTransitForwarding forwards it onto the bus for out-of-process consumers — in addition
/// to running the in-process handlers below.
/// </summary>
public sealed record OrderPlacedEvent(Guid OrderId, string Customer, decimal Total) : IIntegrationEvent;

/// <summary>In-process handler #1.</summary>
public sealed class LogOrderPlacedHandler(ILogger<LogOrderPlacedHandler> logger)
    : INotificationHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[in-process] order {OrderId} placed by {Customer} ({Total:0.00})",
            notification.OrderId, notification.Customer, notification.Total);
        return Task.CompletedTask;
    }
}

/// <summary>In-process handler #2.</summary>
public sealed class SendConfirmationEmailHandler(ILogger<SendConfirmationEmailHandler> logger)
    : INotificationHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[in-process] confirmation email queued for {Customer}", notification.Customer);
        return Task.CompletedTask;
    }
}

/// <summary>The out-of-process side: a MassTransit consumer reacting to the forwarded event on the bus.</summary>
public sealed class OrderPlacedConsumer(ILogger<OrderPlacedConsumer> logger) : IConsumer<OrderPlacedEvent>
{
    public Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        logger.LogInformation("[bus] consumed OrderPlaced {OrderId} for {Customer}",
            context.Message.OrderId, context.Message.Customer);
        return Task.CompletedTask;
    }
}
