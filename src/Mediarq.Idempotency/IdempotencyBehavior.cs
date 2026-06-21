using Mediarq.Caching;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.Caching.Distributed;

namespace Mediarq.Idempotency;

/// <summary>
/// Pipeline behavior that makes requests implementing <see cref="IIdempotentRequest"/> idempotent: the
/// first run is executed and its result stored by idempotency key in an <see cref="IDistributedCache"/>;
/// subsequent runs with the same key return the stored result without re-running the handler. Inert for
/// request types that are not idempotent.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class IdempotencyBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly bool RequestIsIdempotent = typeof(IIdempotentRequest).IsAssignableFrom(typeof(TRequest));
    private static readonly TimeSpan DefaultRetention = TimeSpan.FromHours(24);

    private readonly IDistributedCache _cache;
    private readonly IMediarqCacheSerializer _serializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The distributed cache used to store results by idempotency key.</param>
    /// <param name="serializer">The serializer used to convert results to and from bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cache"/> or <paramref name="serializer"/> is <see langword="null"/>.</exception>
    public IdempotencyBehavior(IDistributedCache cache, IMediarqCacheSerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(serializer);
        _cache = cache;
        _serializer = serializer;
    }

    /// <summary>Active only for request types that implement <see cref="IIdempotentRequest"/>.</summary>
    public bool IsActive => RequestIsIdempotent;

    /// <inheritdoc />
    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handle);

        var idempotent = (IIdempotentRequest)context.Request!;
        var key = "mediarq:idem:" + idempotent.IdempotencyKey;

        var existing = await _cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (existing is { Length: > 0 })
        {
            var stored = _serializer.Deserialize<TResponse>(existing);
            if (stored is not null)
            {
                return stored;
            }
        }

        var response = await handle().ConfigureAwait(false);

        // Only store a non-null result; a null cannot be round-tripped to mean "already handled".
        if (response is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = idempotent.IdempotencyDuration ?? DefaultRetention,
            };

            await _cache.SetAsync(key, _serializer.Serialize(response), options, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
