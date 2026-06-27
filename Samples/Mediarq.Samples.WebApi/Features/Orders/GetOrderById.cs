using Mediarq.Caching;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Samples.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Reads an order. Implements <see cref="ICacheableRequest"/> so the CachingBehavior memoizes the
/// response: the handler (and the database read) only run on a cache miss.
/// </summary>
public sealed record GetOrderByIdQuery(Guid Id) : IQuery<Result<OrderDto>>, ICacheableRequest
{
    public string CacheKey => $"orders:{Id}";
    public TimeSpan? CacheDuration => TimeSpan.FromSeconds(30);
}

public sealed class GetOrderByIdHandler(AppDbContext db, ILogger<GetOrderByIdHandler> logger)
    : IQueryHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken = default)
    {
        // This line only appears on a cache miss — watch it disappear on the second identical request.
        logger.LogInformation("Reading order {Id} from the database (cache miss)", request.Id);

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        return order is null
            ? Result.Failure<OrderDto>(ResultError.NotFound("Order.NotFound", $"Order {request.Id} was not found."))
            : Result.Success(OrderDto.From(order));
    }
}
