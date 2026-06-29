using System.Collections.Concurrent;
using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;
using Mediarq.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

/// <summary>An <see cref="IPublisher"/> that always fails, to exercise the retry/error path.</summary>
public sealed class ThrowingPublisher : IPublisher
{
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
        => Task.FromException(new InvalidOperationException("publish boom"));
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

    [Fact]
    public async Task ProcessPending_Returns_Zero_When_Nothing_Is_Pending()
    {
        await using var context = NewContext();

        var processed = await OutboxDispatcher.ProcessPendingAsync(context, new RecordingPublisher(), 50, CancellationToken.None);

        processed.Should().Be(0);
    }

    [Fact]
    public async Task ProcessPending_Keeps_The_Message_Pending_And_Records_The_Error_When_Publishing_Fails()
    {
        await using var context = NewContext();
        var outbox = new EfCoreOutbox<OutboxTestDbContext>(context);
        outbox.Enqueue(new OrderPlaced(3));
        await context.SaveChangesAsync();

        var processed = await OutboxDispatcher.ProcessPendingAsync(context, new ThrowingPublisher(), 50, CancellationToken.None);

        processed.Should().Be(0);
        var message = await context.Set<OutboxMessage>().SingleAsync();
        message.ProcessedOnUtc.Should().BeNull();
        message.Attempts.Should().Be(1);
        message.Error.Should().Contain("publish boom");
    }

    [Fact]
    public async Task ProcessPending_Records_An_Error_When_The_Message_Type_Cannot_Be_Resolved()
    {
        await using var context = NewContext();
        context.Set<OutboxMessage>().Add(new OutboxMessage { Type = "Does.Not.Exist, Nope", Payload = "{}" });
        await context.SaveChangesAsync();

        var processed = await OutboxDispatcher.ProcessPendingAsync(context, new RecordingPublisher(), 50, CancellationToken.None);

        processed.Should().Be(0);
        var message = await context.Set<OutboxMessage>().SingleAsync();
        message.ProcessedOnUtc.Should().BeNull();
        message.Attempts.Should().Be(1);
        message.Error.Should().Contain("Does.Not.Exist");
    }

    [Fact]
    public async Task ProcessPending_Honors_The_Batch_Size()
    {
        await using var context = NewContext();
        var outbox = new EfCoreOutbox<OutboxTestDbContext>(context);
        outbox.Enqueue(new OrderPlaced(1));
        outbox.Enqueue(new OrderPlaced(2));
        outbox.Enqueue(new OrderPlaced(3));
        await context.SaveChangesAsync();

        var publisher = new RecordingPublisher();
        var processed = await OutboxDispatcher.ProcessPendingAsync(context, publisher, batchSize: 2, CancellationToken.None);

        processed.Should().Be(2);
        publisher.Published.Should().HaveCount(2);
        var pending = await context.Set<OutboxMessage>().CountAsync(m => m.ProcessedOnUtc == null);
        pending.Should().Be(1);
    }

    [Fact]
    public async Task ProcessPending_Publishes_Oldest_First()
    {
        await using var context = NewContext();
        var set = context.Set<OutboxMessage>();
        set.Add(new OutboxMessage
        {
            Type = typeof(OrderPlaced).AssemblyQualifiedName!,
            Payload = System.Text.Json.JsonSerializer.Serialize(new OrderPlaced(20)),
            OccurredOnUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
        });
        set.Add(new OutboxMessage
        {
            Type = typeof(OrderPlaced).AssemblyQualifiedName!,
            Payload = System.Text.Json.JsonSerializer.Serialize(new OrderPlaced(10)),
            OccurredOnUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        });
        await context.SaveChangesAsync();

        var publisher = new RecordingPublisher();
        await OutboxDispatcher.ProcessPendingAsync(context, publisher, 50, CancellationToken.None);

        publisher.Published.Cast<OrderPlaced>().Select(o => o.OrderId).Should().ContainInOrder(10, 20);
    }

    [Fact]
    public void Enqueue_Rejects_A_Null_Notification()
    {
        using var context = NewContext();
        var outbox = new EfCoreOutbox<OutboxTestDbContext>(context);

        var act = () => outbox.Enqueue<OrderPlaced>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("notification");
    }

    [Fact]
    public void EfCoreOutbox_Constructor_Rejects_A_Null_Context()
    {
        var act = () => new EfCoreOutbox<OutboxTestDbContext>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("context");
    }

    [Fact]
    public void ApplyMediarqOutbox_Rejects_A_Null_ModelBuilder()
    {
        var act = () => ((ModelBuilder)null!).ApplyMediarqOutbox();

        act.Should().Throw<ArgumentNullException>().WithParameterName("modelBuilder");
    }

    [Fact]
    public void OutboxOptions_Have_Sensible_Defaults()
    {
        var options = new OutboxOptions();

        options.PollingInterval.Should().Be(TimeSpan.FromSeconds(10));
        options.BatchSize.Should().Be(50);
    }

    [Fact]
    public void AddMediarqOutbox_Registers_The_Outbox_Processor_And_Options()
    {
        var services = new ServiceCollection();
        services.AddDbContext<OutboxTestDbContext>(o => o.UseInMemoryDatabase("reg"));

        services.AddMediarqOutbox<OutboxTestDbContext>(o => o.BatchSize = 7);

        services.Should().Contain(d => d.ServiceType == typeof(IOutbox)
            && d.ImplementationType == typeof(EfCoreOutbox<OutboxTestDbContext>)
            && d.Lifetime == ServiceLifetime.Scoped);
        services.Should().Contain(d => d.ServiceType == typeof(IHostedService));

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<OutboxOptions>().BatchSize.Should().Be(7);
    }

    [Fact]
    public void AddMediarqOutbox_Rejects_A_Null_ServiceCollection()
    {
        var act = () => ((IServiceCollection)null!).AddMediarqOutbox<OutboxTestDbContext>();

        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    [Fact]
    public async Task OutboxProcessor_Publishes_Pending_Messages_While_Running()
    {
        var publisher = new RecordingPublisher();
        var services = new ServiceCollection();
        services.AddSingleton<IPublisher>(publisher);
        services.AddDbContext<OutboxTestDbContext>(o => o.UseInMemoryDatabase("processor-run"));
        services.AddMediarqOutbox<OutboxTestDbContext>(o => o.PollingInterval = TimeSpan.FromMilliseconds(20));

        using var provider = services.BuildServiceProvider();

        using (var scope = provider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OutboxTestDbContext>();
            scope.ServiceProvider.GetRequiredService<IOutbox>().Enqueue(new OrderPlaced(99));
            await context.SaveChangesAsync();
        }

        var processor = provider.GetServices<IHostedService>().OfType<OutboxProcessor<OutboxTestDbContext>>().Single();
        await processor.StartAsync(CancellationToken.None);

        try
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (publisher.Published.IsEmpty && DateTime.UtcNow < deadline)
            {
                await Task.Delay(20);
            }
        }
        finally
        {
            await processor.StopAsync(CancellationToken.None);
        }

        publisher.Published.Should().ContainSingle().Which.Should().BeOfType<OrderPlaced>()
            .Which.OrderId.Should().Be(99);
    }

    [Fact]
    public void OutboxProcessor_Constructor_Rejects_Null_Dependencies()
    {
        var services = new ServiceCollection();
        using var provider = services.BuildServiceProvider();
        var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        var nullScopeFactory = () => new OutboxProcessor<OutboxTestDbContext>(null!, new OutboxOptions());
        var nullOptions = () => new OutboxProcessor<OutboxTestDbContext>(scopeFactory, null!);

        nullScopeFactory.Should().Throw<ArgumentNullException>().WithParameterName("scopeFactory");
        nullOptions.Should().Throw<ArgumentNullException>().WithParameterName("options");
    }
}
