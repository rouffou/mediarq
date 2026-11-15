using Mediarq.Caching;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Results;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;

namespace Mediarq.Idempotency;

/// <summary>
/// Pipeline behavior that makes requests implementing <see cref="IIdempotentRequest"/> idempotent: the
/// first run is executed and its result stored by idempotency key in an <see cref="IDistributedCache"/>;
/// subsequent runs with the same key return the stored result without re-running the handler. Inert for
/// request types that are not idempotent.
/// </summary>
/// <remarks>
/// <para>
/// The cache key is scoped by request type, so two different request types that happen to share an
/// idempotency key never replay each other's result.
/// </para>
/// <para>
/// Failed <see cref="Result"/> values are never stored: a failure is not a completed operation and the
/// caller must be able to retry with the same key.
/// </para>
/// <para>
/// Concurrent calls with the same key inside one process are serialized, so the handler runs once and the
/// others replay its result. Across several instances the check remains best-effort, because
/// <see cref="IDistributedCache"/> has no atomic set-if-absent; use a distributed lock for strict
/// once-only semantics there.
/// </para>
/// </remarks>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type produced by the request.</typeparam>
public sealed class IdempotencyBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>, IConditionalPipelineBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    private static readonly bool RequestIsIdempotent = typeof(IIdempotentRequest).IsAssignableFrom(typeof(TRequest));
    private static readonly TimeSpan DefaultRetention = TimeSpan.FromHours(24);

    // Per-key gates for in-process serialization. Entries are removed when the last waiter leaves.
    private static readonly ConcurrentDictionary<string, KeyGate> Gates = new();

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
        var key = "mediarq:idem:" + typeof(TRequest).FullName + ":" + idempotent.IdempotencyKey;

        if (TryGetStored(await _cache.GetAsync(key, cancellationToken).ConfigureAwait(false), out var cached))
        {
            return cached;
        }

        var gate = AcquireGate(key);
        try
        {
            await gate.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                // Another caller with the same key may have completed while this one was waiting.
                if (TryGetStored(await _cache.GetAsync(key, cancellationToken).ConfigureAwait(false), out var completed))
                {
                    return completed;
                }

                var response = await handle().ConfigureAwait(false);

                // Only store a successful, non-null result: a null cannot be round-tripped to mean
                // "already handled", and a failed Result must stay retryable with the same key.
                if (response is not null && response is not Result { IsFailure: true })
                {
                    var options = new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = idempotent.IdempotencyDuration ?? DefaultRetention,
                    };

                    await _cache.SetAsync(key, _serializer.Serialize(response), options, cancellationToken).ConfigureAwait(false);
                }

                return response;
            }
            finally
            {
                gate.Semaphore.Release();
            }
        }
        finally
        {
            ReleaseGate(key, gate);
        }
    }

    private bool TryGetStored(byte[]? bytes, out TResponse response)
    {
        if (bytes is { Length: > 0 })
        {
            var stored = _serializer.Deserialize<TResponse>(bytes);
            if (stored is not null)
            {
                response = stored;
                return true;
            }
        }

        response = default!;
        return false;
    }

    private static KeyGate AcquireGate(string key)
    {
        while (true)
        {
            var gate = Gates.GetOrAdd(key, static _ => new KeyGate());
            lock (gate)
            {
                if (!gate.Retired)
                {
                    gate.Users++;
                    return gate;
                }
            }

            // The gate was retired between lookup and lock; drop it and retry with a fresh one.
            Gates.TryRemove(new KeyValuePair<string, KeyGate>(key, gate));
        }
    }

    private static void ReleaseGate(string key, KeyGate gate)
    {
        lock (gate)
        {
            if (--gate.Users == 0)
            {
                gate.Retired = true;
                Gates.TryRemove(new KeyValuePair<string, KeyGate>(key, gate));
            }
        }
    }

    private sealed class KeyGate
    {
        public SemaphoreSlim Semaphore { get; } = new(1, 1);

        public int Users;

        public bool Retired;
    }
}
