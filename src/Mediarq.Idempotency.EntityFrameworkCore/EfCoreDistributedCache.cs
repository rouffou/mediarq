using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Mediarq.Idempotency.EntityFrameworkCore;

/// <summary>
/// <see cref="IDistributedCache"/> backed by an EF Core <typeparamref name="TContext"/>, so
/// <c>Mediarq.Idempotency</c>'s <c>IdempotencyBehavior</c> can persist replay results in your own
/// database instead of requiring Redis.
/// </summary>
/// <typeparam name="TContext">The EF Core <see cref="DbContext"/> that maps <see cref="IdempotencyCacheEntry"/>.</typeparam>
/// <remarks>
/// Registered <b>scoped</b> (see <see cref="IdempotencyCacheServiceCollectionExtensions.AddMediarqIdempotencyEntityFrameworkCore{TContext}"/>),
/// tied to the same <typeparamref name="TContext"/> instance as the rest of the request — unlike the
/// usual singleton <c>IDistributedCache</c> implementations (in-memory, Redis). Do not resolve it
/// outside a scope, and do not share it with singleton consumers unrelated to Mediarq.Idempotency.
/// </remarks>
public sealed class EfCoreDistributedCache<TContext> : IDistributedCache
    where TContext : DbContext
{
    private readonly TContext _context;

    /// <summary>Initializes a new instance wrapping <paramref name="context"/>.</summary>
    /// <param name="context">The EF Core context that holds the idempotency cache table.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
    public EfCoreDistributedCache(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc />
    public byte[]? Get(string key) => GetAsync(key).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var entry = await FindAsync(key, token).ConfigureAwait(false);
        if (entry is null)
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow;
        if (entry.ExpiresAtUtc is { } expiresAt && expiresAt <= now)
        {
            _context.Set<IdempotencyCacheEntry>().Remove(entry);
            await _context.SaveChangesAsync(token).ConfigureAwait(false);
            return null;
        }

        if (entry.SlidingExpiration is { } sliding)
        {
            var renewed = now + sliding;
            entry.ExpiresAtUtc = entry.AbsoluteExpiration is { } absolute && renewed > absolute ? absolute : renewed;
            await _context.SaveChangesAsync(token).ConfigureAwait(false);
        }

        return entry.Value;
    }

    /// <inheritdoc />
    public void Refresh(string key) => RefreshAsync(key).GetAwaiter().GetResult();

    /// <inheritdoc />
    public Task RefreshAsync(string key, CancellationToken token = default) =>
        // Reading already applies the sliding-expiration renewal as a side effect.
        GetAsync(key, token);

    /// <inheritdoc />
    public void Remove(string key) => RemoveAsync(key).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        var entry = await FindAsync(key, token).ConfigureAwait(false);
        if (entry is null)
        {
            return;
        }

        _context.Set<IdempotencyCacheEntry>().Remove(entry);
        await _context.SaveChangesAsync(token).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options) =>
        SetAsync(key, value, options).GetAwaiter().GetResult();

    /// <inheritdoc />
    public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(options);

        var now = DateTimeOffset.UtcNow;
        var absolute = options.AbsoluteExpiration;
        if (options.AbsoluteExpirationRelativeToNow is { } relativeToNow)
        {
            absolute = now + relativeToNow;
        }

        var expiresAt = absolute ?? (options.SlidingExpiration is { } sliding ? now + sliding : (DateTimeOffset?)null);

        var entry = await FindAsync(key, token).ConfigureAwait(false);
        if (entry is null)
        {
            entry = new IdempotencyCacheEntry { Key = key };
            _context.Set<IdempotencyCacheEntry>().Add(entry);
        }

        entry.Value = value;
        entry.AbsoluteExpiration = absolute;
        entry.SlidingExpiration = options.SlidingExpiration;
        entry.ExpiresAtUtc = expiresAt;

        await _context.SaveChangesAsync(token).ConfigureAwait(false);
    }

    private Task<IdempotencyCacheEntry?> FindAsync(string key, CancellationToken cancellationToken) =>
        _context.Set<IdempotencyCacheEntry>().FirstOrDefaultAsync(e => e.Key == key, cancellationToken);
}
