namespace Mediarq.Saga;

/// <summary>
/// Persists <typeparamref name="TState"/> instances keyed by <see cref="ISagaState.CorrelationId"/>.
/// The default <see cref="InMemorySagaStore{TState}"/> registered by
/// <see cref="SagaServiceCollectionExtensions.AddMediarqSaga{TState}"/> only survives for the process
/// lifetime — register your own (e.g. backed by a database) before it in production so saga state
/// survives a restart.
/// </summary>
/// <typeparam name="TState">The saga state type.</typeparam>
public interface ISagaStore<TState>
    where TState : ISagaState
{
    /// <summary>Looks up the saga instance correlated to <paramref name="correlationId"/>, if any.</summary>
    /// <param name="correlationId">The correlation id shared by every notification of this saga instance.</param>
    /// <param name="cancellationToken">A token to cancel the lookup.</param>
    /// <returns>The persisted state, or <see langword="null"/> if no instance is correlated to this id yet.</returns>
    Task<TState?> FindAsync(Guid correlationId, CancellationToken cancellationToken = default);

    /// <summary>Persists <paramref name="state"/>, creating or overwriting the instance for its correlation id.</summary>
    /// <param name="state">The state to persist.</param>
    /// <param name="cancellationToken">A token to cancel the save.</param>
    Task SaveAsync(TState state, CancellationToken cancellationToken = default);

    /// <summary>Removes the saga instance correlated to <paramref name="correlationId"/>, if any.</summary>
    /// <param name="correlationId">The correlation id of the instance to remove.</param>
    /// <param name="cancellationToken">A token to cancel the delete.</param>
    Task DeleteAsync(Guid correlationId, CancellationToken cancellationToken = default);
}
