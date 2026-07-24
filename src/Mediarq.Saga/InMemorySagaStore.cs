using System.Collections.Concurrent;

namespace Mediarq.Saga;

/// <summary>
/// Process-lifetime <see cref="ISagaStore{TState}"/> backed by a <see cref="ConcurrentDictionary{TKey,TValue}"/>.
/// State is lost on restart — swap in a persistent implementation (e.g. backed by
/// <c>Mediarq.EntityFrameworkCore</c>) for production use.
/// </summary>
/// <typeparam name="TState">The saga state type.</typeparam>
public sealed class InMemorySagaStore<TState> : ISagaStore<TState>
    where TState : ISagaState
{
    private readonly ConcurrentDictionary<Guid, TState> _instances = new();

    /// <inheritdoc />
    public Task<TState?> FindAsync(Guid correlationId, CancellationToken cancellationToken = default)
        => Task.FromResult(_instances.GetValueOrDefault(correlationId));

    /// <inheritdoc />
    public Task SaveAsync(TState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        _instances[state.CorrelationId] = state;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        _instances.TryRemove(correlationId, out _);
        return Task.CompletedTask;
    }
}
