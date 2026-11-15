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
    /// Publishes up to <paramref name="batchSize"/> pending messages (oldest first). Messages that already
    /// used <paramref name="maxAttempts"/> attempts are skipped. Each message is claimed (its
    /// <see cref="OutboxMessage.Attempts"/> moves from N to N + 1 under an optimistic concurrency check, so a
    /// concurrent processor cannot publish it too), published, then saved on its own: a crash mid-batch
    /// never re-publishes the messages already done. Returns the number successfully published.
    /// </summary>
    public static async Task<int> ProcessPendingAsync(DbContext context, IPublisher publisher, int batchSize, CancellationToken cancellationToken, int maxAttempts = 10)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(publisher);

        var pending = await context.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null && m.Attempts < maxAttempts)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var processed = 0;
        foreach (var message in pending)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await TryClaimAsync(context, message, cancellationToken).ConfigureAwait(false))
            {
                continue;
            }

            try
            {
                await PublishAsync(publisher, message, cancellationToken).ConfigureAwait(false);
                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
                processed++;
            }
            catch (OperationCanceledException)
            {
                // Shutdown is not a failed attempt: give the attempt back so the message is retried in full.
                message.Attempts--;
                await context.SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
                throw;
            }
            catch (Exception ex)
            {
                // Leave the message pending (until MaxAttempts); the attempt was already counted by the claim.
                message.Error = ex.Message;
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return processed;
    }

    private static async Task<bool> TryClaimAsync(DbContext context, OutboxMessage message, CancellationToken cancellationToken)
    {
        message.Attempts++;
        try
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another processor claimed (or finished) this message first: forget our stale copy and move on.
            context.Entry(message).State = EntityState.Detached;
            return false;
        }
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
