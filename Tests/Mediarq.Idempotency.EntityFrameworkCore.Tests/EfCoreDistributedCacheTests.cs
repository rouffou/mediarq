using FluentAssertions;
using Mediarq.Idempotency.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Mediarq.Idempotency.EntityFrameworkCore.Tests;

public sealed class IdempotencyTestDbContext(DbContextOptions<IdempotencyTestDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyMediarqIdempotencyCache();
}

public class EfCoreDistributedCacheTests
{
    private static IdempotencyTestDbContext NewContext()
        => new(new DbContextOptionsBuilder<IdempotencyTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task SetAsync_Then_GetAsync_Roundtrips_The_Value()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        await cache.SetAsync("key-1", [1, 2, 3], new DistributedCacheEntryOptions());
        var value = await cache.GetAsync("key-1");

        value.Should().Equal(1, 2, 3);
    }

    [Fact]
    public async Task GetAsync_Returns_Null_For_An_Unknown_Key()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        var value = await cache.GetAsync("missing");

        value.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Returns_Null_And_Evicts_An_Expired_Entry()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        await cache.SetAsync("key-1", [1], new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1),
        });
        await Task.Delay(20);

        var value = await cache.GetAsync("key-1");

        value.Should().BeNull();
        (await context.Set<IdempotencyCacheEntry>().AnyAsync(e => e.Key == "key-1")).Should().BeFalse();
    }

    [Fact]
    public async Task SetAsync_Overwrites_An_Existing_Entry()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        await cache.SetAsync("key-1", [1], new DistributedCacheEntryOptions());
        await cache.SetAsync("key-1", [2, 2], new DistributedCacheEntryOptions());

        var value = await cache.GetAsync("key-1");
        value.Should().Equal(2, 2);
        (await context.Set<IdempotencyCacheEntry>().CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task RemoveAsync_Deletes_The_Entry()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        await cache.SetAsync("key-1", [1], new DistributedCacheEntryOptions());
        await cache.RemoveAsync("key-1");

        (await cache.GetAsync("key-1")).Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Extends_ExpiresAtUtc_Under_A_Sliding_Expiration()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        await cache.SetAsync("key-1", [1], new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10),
        });
        var firstExpiry = (await context.Set<IdempotencyCacheEntry>().SingleAsync()).ExpiresAtUtc;

        await Task.Delay(20);
        await cache.GetAsync("key-1");
        var secondExpiry = (await context.Set<IdempotencyCacheEntry>().SingleAsync()).ExpiresAtUtc;

        secondExpiry.Should().BeAfter(firstExpiry!.Value);
    }

    [Fact]
    public void Sync_Get_Set_Remove_Delegate_To_The_Async_Members()
    {
        using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

        cache.Set("key-1", [9], new DistributedCacheEntryOptions());
        cache.Get("key-1").Should().Equal(9);

        cache.Remove("key-1");
        cache.Get("key-1").Should().BeNull();
    }
}
