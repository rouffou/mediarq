using Mediarq.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Samples.WebApi.Domain;

/// <summary>
/// The application's EF Core context. It doubles as the Mediarq unit of work (see
/// <c>AddMediarqEntityFrameworkCore&lt;AppDbContext&gt;()</c>) and hosts the transactional outbox table
/// (mapped by <see cref="OutboxModelBuilderExtensions.ApplyMediarqOutbox"/>).
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>().OwnsMany(o => o.Items);

        // Maps the OutboxMessage entity so events can be staged in the same transaction as the order.
        modelBuilder.ApplyMediarqOutbox();
    }
}
