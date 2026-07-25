using FluentAssertions;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Mediarq.EntityFrameworkCore.Tests;

public class DomainEventsInterceptorTests
{
    public sealed record OrderPlaced(int OrderId) : INotification;

    public sealed class Order : AggregateRoot
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";

        public void Place()
        {
            AddDomainEvent(new OrderPlaced(Id));
        }
    }

    public sealed class DomainEventsDbContext(DbContextOptions<DomainEventsDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders => Set<Order>();
    }

    // Chained after DomainEventsInterceptor to deterministically force SaveChanges to fail, without
    // depending on any provider-specific constraint-violation behavior.
    private sealed class ThrowingInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Simulated SaveChanges failure.");
    }

    private static DomainEventsDbContext NewContext(string dbName, DomainEventsInterceptor interceptor, params IInterceptor[] others) =>
        new(new DbContextOptionsBuilder<DomainEventsDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddInterceptors([interceptor, .. others])
            .Options);

    // Mediator.Publish<TNotification> resolves handlers from notification.GetType() at runtime, not from
    // the TNotification generic argument, so it dispatches correctly to INotificationHandler<OrderPlaced>
    // no matter what static type the interceptor's call site uses. But Moq's Verify() DOES match on the
    // exact closed generic method invoked — the interceptor's foreach loop variable is statically typed
    // INotification, so the call is Publish<INotification>, not Publish<OrderPlaced>. Match that shape here.
    private static void VerifyPublished(Mock<IPublisher> publisher, int orderId, Times times) =>
        publisher.Verify(
            p => p.Publish(It.Is<INotification>(n => (n as OrderPlaced)!.OrderId == orderId), It.IsAny<CancellationToken>()),
            times);

    private static void VerifyPublishedCount(Mock<IPublisher> publisher, Times times) =>
        publisher.Verify(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), times);

    [Fact]
    public async Task SaveChangesAsync_Publishes_Domain_Events_After_A_Successful_Commit()
    {
        var publisher = new Mock<IPublisher>();
        var interceptor = new DomainEventsInterceptor(publisher.Object);
        await using var context = NewContext(Guid.NewGuid().ToString(), interceptor);

        var order = new Order { Id = 1, Code = "A" };
        order.Place();
        context.Orders.Add(order);

        await context.SaveChangesAsync();

        VerifyPublished(publisher, orderId: 1, Times.Once());
    }

    [Fact]
    public async Task SaveChangesAsync_Clears_Domain_Events_So_A_Later_Commit_Does_Not_Republish()
    {
        var publisher = new Mock<IPublisher>();
        var interceptor = new DomainEventsInterceptor(publisher.Object);
        var dbName = Guid.NewGuid().ToString();
        await using var context = NewContext(dbName, interceptor);

        var order = new Order { Id = 1, Code = "A" };
        order.Place();
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        order.DomainEvents.Should().BeEmpty();

        order.Code = "A-updated";
        await context.SaveChangesAsync(); // no new domain event raised

        VerifyPublishedCount(publisher, Times.Once());
    }

    [Fact]
    public async Task SaveChangesAsync_Publishes_Events_From_Every_Aggregate_With_Pending_Events()
    {
        var publisher = new Mock<IPublisher>();
        var interceptor = new DomainEventsInterceptor(publisher.Object);
        await using var context = NewContext(Guid.NewGuid().ToString(), interceptor);

        var order1 = new Order { Id = 1, Code = "A" };
        var order2 = new Order { Id = 2, Code = "B" };
        order1.Place();
        order2.Place();
        context.Orders.AddRange(order1, order2);

        await context.SaveChangesAsync();

        VerifyPublishedCount(publisher, Times.Exactly(2));
    }

    [Fact]
    public async Task SaveChangesAsync_Does_Not_Publish_When_No_Aggregate_Has_Domain_Events()
    {
        var publisher = new Mock<IPublisher>();
        var interceptor = new DomainEventsInterceptor(publisher.Object);
        await using var context = NewContext(Guid.NewGuid().ToString(), interceptor);

        context.Orders.Add(new Order { Id = 1, Code = "A" }); // no .Place()
        await context.SaveChangesAsync();

        VerifyPublishedCount(publisher, Times.Never());
    }

    [Fact]
    public async Task A_Failed_Commit_Does_Not_Publish_And_Does_Not_Leak_State_Into_The_Next_Commit()
    {
        var publisher = new Mock<IPublisher>();
        var interceptor = new DomainEventsInterceptor(publisher.Object);
        var dbName = Guid.NewGuid().ToString();

        await using (var failingContext = NewContext(dbName, interceptor, new ThrowingInterceptor()))
        {
            var order = new Order { Id = 1, Code = "A" };
            order.Place();
            failingContext.Orders.Add(order);

            var act = async () => await failingContext.SaveChangesAsync();
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        VerifyPublishedCount(publisher, Times.Never());

        // A fresh, non-throwing context against the same database: only its own event is published --
        // nothing leaked from the failed attempt above.
        await using var nextContext = NewContext(dbName, interceptor);
        var order2 = new Order { Id = 2, Code = "B" };
        order2.Place();
        nextContext.Orders.Add(order2);
        await nextContext.SaveChangesAsync();

        VerifyPublished(publisher, orderId: 2, Times.Once());
        VerifyPublishedCount(publisher, Times.Once());
    }

    [Fact]
    public void Constructor_Throws_On_Null_Publisher()
    {
        var act = () => new DomainEventsInterceptor(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqDomainEvents_Registers_The_Interceptor_As_Scoped()
    {
        var services = new ServiceCollection();

        services.AddMediarqDomainEvents();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IInterceptor)
            && d.ImplementationType == typeof(DomainEventsInterceptor)
            && d.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddMediarqDomainEvents_Throws_On_Null_Services()
    {
        IServiceCollection services = null!;

        ((Action)(() => services.AddMediarqDomainEvents())).Should().Throw<ArgumentNullException>();
    }
}
