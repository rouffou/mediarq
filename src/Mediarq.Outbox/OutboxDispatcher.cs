using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Outbox;

/// <summary>
/// Reads pending <see cref="OutboxMessage"/> rows and publishes them through the Mediarq
/// <see cref="IPublisher"/>. Reflection is used to deserialize the stored payload to its CLR type and to
/// invoke the generic <see cref="IPublisher.Publish{TNotification}"/> — both cached per type.
/// </summary>
internal static class OutboxDispatcher
{
    private static readonly ConcurrentDictionary<string, Type?> TypeCache = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> PublishMethods = new();
    private static readonly MethodInfo PublishOpenMethod =
        typeof(IPublisher).GetMethod(nameof(IPublisher.Publish))
        ?? throw new InvalidOperationException("IPublisher.Publish method not found.");

    /// <summary>
    /// Publishes up to <paramref name="batchSize"/> pending messages (oldest first), marking each
    /// processed or recording the error, then saves. Returns the number successfully published.
    /// </summary>
    public static async Task<int> ProcessPendingAsync(DbContext context, IPublisher publisher, int batchSize, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(publisher);

        var pending = await context.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (pending.Count == 0)
        {
            return 0;
        }

        var processed = 0;
        foreach (var message in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await PublishAsync(publisher, message, cancellationToken).ConfigureAwait(false);
                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
                processed++;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Leave the message pending; it will be retried on the next pass.
                message.Attempts++;
                message.Error = ex.Message;
            }
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return processed;
    }

    private static Task PublishAsync(IPublisher publisher, OutboxMessage message, CancellationToken cancellationToken)
    {
        var type = TypeCache.GetOrAdd(message.Type, static t => Type.GetType(t, throwOnError: false))
            ?? throw new InvalidOperationException($"Could not resolve outbox message type '{message.Type}'.");

        var notification = JsonSerializer.Deserialize(message.Payload, type, OutboxJson.Options)
            ?? throw new InvalidOperationException($"Could not deserialize outbox message '{message.Id}'.");

        var publish = PublishMethods.GetOrAdd(type, static t => PublishOpenMethod.MakeGenericMethod(t));
        return (Task)publish.Invoke(publisher, [notification, cancellationToken])!;
    }
}
