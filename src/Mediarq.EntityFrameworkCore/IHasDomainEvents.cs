using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.EntityFrameworkCore;

/// <summary>
/// Implemented by an aggregate root that raises domain events to be published after a successful
/// <c>SaveChanges</c> commit. Collected and cleared by <see cref="DomainEventsInterceptor"/> for every
/// tracked entity implementing this interface.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>The domain events raised by this aggregate since the last commit.</summary>
    IReadOnlyCollection<INotification> DomainEvents { get; }

    /// <summary>Clears the raised domain events, so they are not published again on a later commit.</summary>
    void ClearDomainEvents();
}
