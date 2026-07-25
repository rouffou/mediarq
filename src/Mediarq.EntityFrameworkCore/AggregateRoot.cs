using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.EntityFrameworkCore;

/// <summary>
/// Convenience base class implementing <see cref="IHasDomainEvents"/>: derive your aggregate root from
/// this and call <see cref="AddDomainEvent"/> when something worth publishing happens. Not required —
/// implement <see cref="IHasDomainEvents"/> directly if your aggregate already has a different base class.
/// </summary>
public abstract class AggregateRoot : IHasDomainEvents
{
    private readonly List<INotification> _domainEvents = [];

    /// <inheritdoc />
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents;

    /// <summary>Raises a domain event, to be published after the current unit of work commits.</summary>
    /// <param name="domainEvent">The event to raise.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainEvent"/> is <see langword="null"/>.</exception>
    protected void AddDomainEvent(INotification domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        _domainEvents.Add(domainEvent);
    }

    /// <inheritdoc />
    public void ClearDomainEvents() => _domainEvents.Clear();
}
