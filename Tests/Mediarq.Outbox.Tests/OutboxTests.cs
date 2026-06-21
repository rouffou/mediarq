using System.Collections.Concurrent;
using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;
using Mediarq.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Outbox.Tests;

public sealed record OrderPlaced(int OrderId) : INotification;

public sealed class OutboxTestDbContext(DbContextOptions<OutboxTestDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyMediarqOutbox();
}

/// <summary>Minimal <see cref="IPublisher"/> that records what it publishes.</summary>
public sealed class RecordingPublisher : IPublisher
{
    public ConcurrentQueue<INotification> Published { get; } = new();

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        Published.Enqueue(notification);
        return Task.CompletedTask;
    }
}

public class OutboxTests
{
    private static OutboxTestDbContext NewContext()
        => new(new DbContextOptionsBuilder<OutboxTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Enqueue_Persists_A_Pending_OutboxMessage()
    {
        await using var context = NewContext();
        var outbox = new EfCoreOutbox<OutboxTestDbContext>(context);

        outbox.Enqueue(new OrderPlaced(42));
        await context.SaveChangesAsync();

        var message = await context.Set<OutboxMessage>().SingleAsync();
        message.ProcessedOnUtc.Should().BeNull();
        message.Type.Should().Contain(nameof(OrderPlaced));
        message.Payload.Should().Contain("42");
    }

    [Fact]
    public async Task ProcessPending_Publishes_The_Notification_And_Marks_It_Processed()
    {
        await using var context = NewContext();
        var outbox = new EfCoreOutbox<OutboxTestDbContext>(context);
        outbox.Enqueue(new OrderPlaced(7));
        await context.SaveChangesAsync();

        var publisher = new RecordingPublisher();

        var processed = await OutboxDispatcher.ProcessPendingAsync(context, publisher, batchSize: 50, CancellationToken.None);

        processed.Should().Be(1);
        publisher.Published.Should().ContainSingle().Which.Should().BeOfType<OrderPlaced>()
            .Which.OrderId.Should().Be(7);

        var message = await context.Set<OutboxMessage>().SingleAsync();
        message.ProcessedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task ProcessPending_Does_Not_Republish_Already_Processed_Messages()
    {
        await using var context = NewContext();
        var outbox = new EfCoreOutbox<OutboxTestDbContext>(context);
        outbox.Enqueue(new OrderPlaced(1));
        await context.SaveChangesAsync();

        var publisher = new RecordingPublisher();

        var firstPass = await OutboxDispatcher.ProcessPendingAsync(context, publisher, 50, CancellationToken.None);
        var secondPass = await OutboxDispatcher.ProcessPendingAsync(context, publisher, 50, CancellationToken.None);

        firstPass.Should().Be(1);
        secondPass.Should().Be(0);
        publisher.Published.Should().HaveCount(1);
    }
}
