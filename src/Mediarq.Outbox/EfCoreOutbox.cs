using System.Text.Json;
using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Outbox;

/// <summary>Shared JSON options for outbox payloads.</summary>
internal static class OutboxJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}

/// <summary>
/// <see cref="IOutbox"/> backed by an EF Core <typeparamref name="TContext"/>. Enqueued notifications
/// are added to the context's change tracker and persisted by the next <c>SaveChanges</c>, so they are
/// stored in the same transaction as your business data.
/// </summary>
/// <typeparam name="TContext">The EF Core <see cref="DbContext"/> that maps <see cref="OutboxMessage"/>.</typeparam>
public sealed class EfCoreOutbox<TContext> : IOutbox
    where TContext : DbContext
{
    private readonly TContext _context;

    /// <summary>Initializes a new instance wrapping <paramref name="context"/>.</summary>
    /// <param name="context">The EF Core context that holds the outbox table.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
    public EfCoreOutbox(TContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc />
    public void Enqueue<TNotification>(TNotification notification)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        // Serialize using the runtime type so derived notifications round-trip fully.
        var runtimeType = notification.GetType();

        var message = new OutboxMessage
        {
            Type = runtimeType.AssemblyQualifiedName ?? runtimeType.FullName ?? runtimeType.Name,
            Payload = JsonSerializer.Serialize(notification, runtimeType, OutboxJson.Options),
            OccurredOnUtc = DateTime.UtcNow,
        };

        _context.Set<OutboxMessage>().Add(message);
    }
}
