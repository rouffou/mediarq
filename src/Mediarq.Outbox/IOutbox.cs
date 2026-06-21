using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Outbox;

/// <summary>
/// Enqueues notifications into the transactional outbox. The message is added to the current EF Core
/// change tracker but not committed — it is persisted when your unit of work calls <c>SaveChanges</c>,
/// so the event is stored atomically with your business data. The <see cref="OutboxProcessor{TContext}"/>
/// publishes it afterwards.
/// </summary>
public interface IOutbox
{
    /// <summary>
    /// Stages <paramref name="notification"/> for reliable publication. Call your unit of work's
    /// <c>SaveChanges</c> to commit it together with the rest of the transaction.
    /// </summary>
    /// <typeparam name="TNotification">The notification type.</typeparam>
    /// <param name="notification">The notification to enqueue.</param>
    void Enqueue<TNotification>(TNotification notification)
        where TNotification : INotification;
}
