using System.Threading.RateLimiting;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.RateLimiting;

/// <summary>
/// Pipeline behavior that acquires a permit from the named policy of <see cref="IRateLimitedRequest"/>
/// requests before running the handler, throwing <see cref="RateLimitExceededException"/> when no permit
/// is available. Inert for request types that are not <see cref="IRateLimitedRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class RateLimitingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly bool RequestIsRateLimited = typeof(IRateLimitedRequest).IsAssignableFrom(typeof(TRequest));

    private readonly RateLimiterRegistry _registry;

    /// <summary>Initializes a new instance resolving policies from <paramref name="registry"/>.</summary>
    /// <param name="registry">The registry of named rate limit policies.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/> is <see langword="null"/>.</exception>
    public RateLimitingBehavior(RateLimiterRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        _registry = registry;
    }

    /// <summary>Active only for request types that implement <see cref="IRateLimitedRequest"/>.</summary>
    public bool IsActive => RequestIsRateLimited;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var rateLimited = (IRateLimitedRequest)context.Request!;
        var partitionKey = rateLimited.PartitionKey ?? "*";
        var limiter = _registry.GetPolicy(rateLimited.PolicyName);

        using var lease = await limiter.AcquireAsync(partitionKey, permitCount: 1, cancellationToken).ConfigureAwait(false);

        if (!lease.IsAcquired)
        {
            TimeSpan? retryAfter = lease.TryGetMetadata(MetadataName.RetryAfter, out var value) ? value : null;
            throw new RateLimitExceededException(rateLimited.PolicyName, partitionKey, retryAfter);
        }

        return await handle().ConfigureAwait(false);
    }
}
