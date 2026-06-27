using System.Runtime.CompilerServices;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Samples.WebApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>A streaming request: the endpoint returns the items as they are produced.</summary>
public sealed record StreamOrdersRequest : IStreamRequest<OrderDto>;

public sealed class StreamOrdersHandler(AppDbContext db)
    : IStreamRequestHandler<StreamOrdersRequest, OrderDto>
{
    public async IAsyncEnumerable<OrderDto> Handle(
        StreamOrdersRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var order in db.Orders.AsAsyncEnumerable().WithCancellation(cancellationToken))
            yield return OrderDto.From(order);
    }
}
