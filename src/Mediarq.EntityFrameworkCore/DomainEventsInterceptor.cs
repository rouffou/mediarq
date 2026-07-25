using System.Collections.Concurrent;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Mediarq.EntityFrameworkCore;

/// <summary>
/// A <c>SaveChanges</c> interceptor that collects domain events staged on tracked
/// <see cref="IHasDomainEvents"/> aggregates and publishes them after a successful commit.
/// </summary>
/// <remarks>
/// Events are collected and cleared from their aggregates <em>before</em> <c>SaveChanges</c> runs (so a
/// concurrent read never observes a half-published state, and a retried save never re-collects the same
/// events), then published only once the commit actually succeeds. If <c>SaveChanges</c> fails, the
/// collected events for that context are discarded — they were already cleared from the aggregates, so a
/// caller that catches the failure and calls <c>SaveChanges</c> again will not accidentally raise them
/// twice; the trade-off is that a failed commit's domain events are lost rather than retried, which is
/// the same "commit implies the events happened" guarantee <c>Mediarq.Outbox</c> gives for cross-process
/// events, at in-process publish granularity.
///
/// Only the asynchronous <c>SaveChangesAsync</c> path is intercepted — <see cref="IPublisher"/> has no
/// synchronous overload, so publishing from the synchronous <c>SaveChanges</c> hooks would require
/// blocking on async work. Call <c>SaveChangesAsync</c> (already the case for Mediarq's own
/// <c>Mediarq.UnitOfWork</c> behavior) to have events published.
/// </remarks>
public sealed class DomainEventsInterceptor : SaveChangesInterceptor
{
    private readonly IPublisher _publisher;
    private readonly ConcurrentDictionary<DbContext, List<INotification>> _pendingEvents = new();

    /// <summary>Initializes a new instance publishing collected events through <paramref name="publisher"/>.</summary>
    /// <param name="publisher">The publisher used to dispatch collected domain events after commit.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="publisher"/> is <see langword="null"/>.</exception>
    public DomainEventsInterceptor(IPublisher publisher)
    {
        ArgumentNullException.ThrowIfNull(publisher);
        _publisher = publisher;
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            var events = CollectAndClearDomainEvents(context);
            if (events.Count > 0)
            {
                _pendingEvents[context] = events;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <inheritdoc />
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context && _pendingEvents.TryRemove(context, out var events))
        {
            foreach (var domainEvent in events)
            {
                await _publisher.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            _pendingEvents.TryRemove(context, out _);
        }

        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private static List<INotification> CollectAndClearDomainEvents(DbContext context)
    {
        var aggregatesWithEvents = context.ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        if (aggregatesWithEvents.Count == 0)
        {
            return [];
        }

        var events = new List<INotification>();
        foreach (var aggregate in aggregatesWithEvents)
        {
            events.AddRange(aggregate.DomainEvents);
            aggregate.ClearDomainEvents();
        }

        return events;
    }
}
