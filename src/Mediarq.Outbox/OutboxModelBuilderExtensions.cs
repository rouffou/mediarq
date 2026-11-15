using Microsoft.EntityFrameworkCore;

namespace Mediarq.Outbox;

/// <summary>
/// EF Core model configuration for the Mediarq outbox.
/// </summary>
public static class OutboxModelBuilderExtensions
{
    /// <summary>
    /// Maps the <see cref="OutboxMessage"/> entity. Call from your <c>DbContext.OnModelCreating</c>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    /// <returns>The same model builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modelBuilder"/> is <see langword="null"/>.</exception>
    public static ModelBuilder ApplyMediarqOutbox(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        var entity = modelBuilder.Entity<OutboxMessage>();
        entity.HasKey(m => m.Id);
        entity.Property(m => m.Type).IsRequired();
        entity.Property(m => m.Payload).IsRequired();
        // Used as an optimistic claim: only one processor instance can move a message from N to N + 1
        // attempts, so concurrent instances never publish the same message twice. No schema change.
        entity.Property(m => m.Attempts).IsConcurrencyToken();
        // Speeds up the "pending messages" query used by the processor.
        entity.HasIndex(m => m.ProcessedOnUtc);

        return modelBuilder;
    }
}
