using FluentAssertions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.EntityFrameworkCore;
using Mediarq.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.EntityFrameworkCore.Tests;

public class EfCoreUnitOfWorkTests
{
    public sealed class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<Item> Items => Set<Item>();
    }

    private static DbContextOptions<TestDbContext> InMemory(string name) =>
        new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(name).Options;

    [Fact]
    public async Task SaveChangesAsync_Persists_Pending_Changes()
    {
        await using var context = new TestDbContext(InMemory(Guid.NewGuid().ToString()));
        context.Items.Add(new Item { Name = "x" });
        var unitOfWork = new EfCoreUnitOfWork<TestDbContext>(context);

        var written = await unitOfWork.SaveChangesAsync();

        written.Should().Be(1);
    }

    [Fact]
    public void AddMediarqEntityFrameworkCore_Registers_UnitOfWork_And_Behavior()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TestDbContext>(o => o.UseInMemoryDatabase("reg"));

        services.AddMediarqEntityFrameworkCore<TestDbContext>();

        services.Should().Contain(d => d.ServiceType == typeof(IUnitOfWork)
            && d.ImplementationType == typeof(EfCoreUnitOfWork<TestDbContext>));
        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(UnitOfWorkBehavior<,>));
    }
}
