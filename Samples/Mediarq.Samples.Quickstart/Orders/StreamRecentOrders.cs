using System.Runtime.CompilerServices;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Samples.Quickstart.Domain;

namespace Mediarq.Samples.Quickstart.Orders;

/// <summary>
/// A streaming request: dispatched with <c>ISender.CreateStream</c> and consumed with
/// <c>await foreach</c>. Items are produced lazily as the handler yields them.
/// </summary>
public sealed record StreamRecentOrders(int Count) : IStreamRequest<Order>;

public sealed class StreamRecentOrdersHandler(IOrderStore store)
    : IStreamRequestHandler<StreamRecentOrders, Order>
{
    public async IAsyncEnumerable<Order> Handle(
        StreamRecentOrders request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var order in store.All().Take(request.Count))
        {
            // Simulate latency between items (a real handler might page a DB or read a queue).
            await Task.Delay(40, cancellationToken);
            yield return order;
        }
    }
}
