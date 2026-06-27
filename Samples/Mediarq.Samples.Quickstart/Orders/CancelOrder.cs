using Mediarq.Core.Common.Requests.Command;
using Mediarq.Samples.Quickstart.Domain;
using Microsoft.Extensions.Logging;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>
/// A no-result (void) command: implements <see cref="ICommand"/> and is dispatched through the very
/// same pipeline as any other request (its response type is <c>Unit</c> under the hood).
/// </summary>
public sealed record CancelOrderCommand(Guid Id) : ICommand;

public sealed class CancelOrderCommandHandler(IOrderStore store, ILogger<CancelOrderCommandHandler> logger)
    : ICommandHandler<CancelOrderCommand>
{
    public Task Handle(CancelOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order = store.Find(request.Id);
        if (order is not null)
        {
            store.Update(order with { Status = OrderStatus.Cancelled });
            logger.LogInformation("Order {Id} cancelled", request.Id);
        }

        return Task.CompletedTask;
    }
}
