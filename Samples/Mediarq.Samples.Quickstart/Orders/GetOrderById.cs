using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.Quickstart.Domain;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>A query that returns the order, or a typed <see cref="ResultError.NotFound"/> failure.</summary>
public sealed record GetOrderByIdQuery(Guid Id) : IQuery<Result<Order>>;

public sealed class GetOrderByIdQueryHandler(IOrderStore store)
    : IQueryHandler<GetOrderByIdQuery, Result<Order>>
{
    public Task<Result<Order>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken = default)
    {
        var order = store.Find(request.Id);

        // No exceptions for expected outcomes: model "not found" as a failed Result with a category.
        return Task.FromResult(order is null
            ? Result.Failure<Order>(ResultError.NotFound("Order.NotFound", $"Order {request.Id} was not found."))
            : Result.Success(order));
    }
}
