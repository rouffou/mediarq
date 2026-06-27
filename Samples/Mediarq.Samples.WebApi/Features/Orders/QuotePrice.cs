using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Mediarq.Polly;

namespace Mediarq.Samples.WebApi.Features.Orders;

/// <summary>
/// Quotes a price by calling a flaky downstream service. Implements <see cref="IResilientRequest"/>, so
/// the ResilienceBehavior runs the handler through the named Polly pipeline ("orders-pricing") — retrying
/// transient failures transparently.
/// </summary>
public sealed record QuotePriceQuery(Guid OrderId) : IQuery<Result<decimal>>, IResilientRequest
{
    public string ResiliencePipelineName => "orders-pricing";
}

public sealed class QuotePriceHandler(IPricingService pricing)
    : IQueryHandler<QuotePriceQuery, Result<decimal>>
{
    public async Task<Result<decimal>> Handle(QuotePriceQuery request, CancellationToken cancellationToken = default)
        => Result.Success(await pricing.QuoteAsync(request.OrderId, cancellationToken));
}

/// <summary>A downstream pricing dependency.</summary>
public interface IPricingService
{
    Task<decimal> QuoteAsync(Guid orderId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Fails two attempts out of every three to simulate a flaky service, so the Polly retry policy is
/// visibly exercised (watch the attempt numbers climb in the logs before a success).
/// </summary>
public sealed class FlakyPricingService(ILogger<FlakyPricingService> logger) : IPricingService
{
    private int _attempts;

    public Task<decimal> QuoteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var attempt = Interlocked.Increment(ref _attempts);
        logger.LogInformation("Pricing attempt #{Attempt} for order {OrderId}", attempt, orderId);

        if (attempt % 3 != 0)
            throw new InvalidOperationException("Pricing service temporarily unavailable.");

        return Task.FromResult(123.45m);
    }
}
