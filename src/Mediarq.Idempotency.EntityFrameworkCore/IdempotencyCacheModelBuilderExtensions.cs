using Microsoft.EntityFrameworkCore;

namespace Mediarq.Idempotency.EntityFrameworkCore;

/// <summary>
/// EF Core model configuration for the Mediarq idempotency cache.
/// </summary>
public static class IdempotencyCacheModelBuilderExtensions
{
    /// <summary>
    /// Maps the <see cref="IdempotencyCacheEntry"/> entity. Call from your <c>DbContext.OnModelCreating</c>.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    /// <returns>The same model builder, enabling fluent chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modelBuilder"/> is <see langword="null"/>.</exception>
    public static ModelBuilder ApplyMediarqIdempotencyCache(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        var entity = modelBuilder.Entity<IdempotencyCacheEntry>();
        entity.HasKey(e => e.Key);
        // 449 matches the max key length SQL Server can index (900 byte limit / typical multi-byte
        // collation); keeps the table indexable across providers without a provider-specific override.
        entity.Property(e => e.Key).HasMaxLength(449);
        entity.Property(e => e.Value).IsRequired();
        // Speeds up the cleanup service's "expired entries" query.
        entity.HasIndex(e => e.ExpiresAtUtc);

        return modelBuilder;
    }
}
