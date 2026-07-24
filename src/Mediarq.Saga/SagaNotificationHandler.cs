using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Saga;

/// <summary>
/// Base class for one step of a saga / process manager: reacts to <typeparamref name="TNotification"/>,
/// loading (or creating) the correlated <typeparamref name="TState"/> from an <see cref="ISagaStore{TState}"/>
/// beforehand and persisting it afterwards. Register one subclass per notification the process reacts to —
/// they all share state through the same correlation id.
/// </summary>
/// <typeparam name="TNotification">The notification this step reacts to.</typeparam>
/// <typeparam name="TState">The saga state type, shared across every step of the process.</typeparam>
/// <param name="store">The store the state is loaded from and saved to.</param>
public abstract class SagaNotificationHandler<TNotification, TState>(ISagaStore<TState> store)
    : INotificationHandler<TNotification>
    where TNotification : INotification
    where TState : class, ISagaState
{
    /// <inheritdoc />
    public async Task Handle(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var correlationId = GetCorrelationId(notification);
        var state = await store.FindAsync(correlationId, cancellationToken).ConfigureAwait(false)
            ?? CreateState(correlationId);

        // A completed instance ignores further notifications correlated to it instead of reprocessing.
        if (state.IsComplete)
        {
            return;
        }

        await HandleAsync(notification, state, cancellationToken).ConfigureAwait(false);

        await store.SaveAsync(state, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Extracts the correlation id from <paramref name="notification"/> — the value every step of the
    /// same process instance must agree on (e.g. an order id).
    /// </summary>
    /// <param name="notification">The notification being handled.</param>
    protected abstract Guid GetCorrelationId(TNotification notification);

    /// <summary>
    /// Creates the initial state the first time a notification correlates to a saga instance the store
    /// has no record of yet.
    /// </summary>
    /// <param name="correlationId">The correlation id resolved by <see cref="GetCorrelationId"/>.</param>
    protected abstract TState CreateState(Guid correlationId);

    /// <summary>
    /// Handles <paramref name="notification"/> against the current <paramref name="state"/>. Mutate
    /// <paramref name="state"/> in place — including setting <see cref="ISagaState.IsComplete"/> once the
    /// process reaches a terminal step — it is persisted automatically once this returns. Enqueue any
    /// follow-up event via <c>Mediarq.Outbox</c>'s <c>IOutbox</c> so the next step is delivered reliably.
    /// </summary>
    /// <param name="notification">The notification being handled.</param>
    /// <param name="state">The current saga state, loaded (or just created) for this correlation id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    protected abstract Task HandleAsync(TNotification notification, TState state, CancellationToken cancellationToken);
}
