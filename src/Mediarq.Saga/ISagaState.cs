namespace Mediarq.Saga;

/// <summary>
/// State persisted by a saga / process manager across the sequence of notifications it reacts to.
/// Implementations are typically a record or a plain class holding whatever data the process needs to
/// remember between steps (e.g. which steps completed, amounts, identifiers collected so far).
/// </summary>
public interface ISagaState
{
    /// <summary>
    /// Identifies which saga instance a notification belongs to (e.g. an order id, a payment id).
    /// Every notification handled by the same saga instance must resolve to the same correlation id.
    /// </summary>
    Guid CorrelationId { get; }

    /// <summary>
    /// Set by the saga once its process has reached a terminal step. Stores backing
    /// <see cref="ISagaStore{TState}"/> may use this to stop returning the instance from
    /// <see cref="ISagaStore{TState}.FindAsync"/>, or to evict it — see the store's own documentation.
    /// </summary>
    bool IsComplete { get; set; }
}
