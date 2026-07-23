using FluentAssertions;
using Mediarq.Idempotency.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Idempotency.EntityFrameworkCore.Tests;

public class IdempotencyCacheCleanupServiceTests
{
    private static ServiceProvider BuildProvider(string databaseName)
    {
        var services = new ServiceCollection();
        services.AddDbContext<IdempotencyTestDbContext>(o => o.UseInMemoryDatabase(databaseName));
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Sweep_Removes_Expired_Entries_But_Keeps_Live_Ones()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using (var provider = BuildProvider(databaseName))
        {
            await using var context = provider.GetRequiredService<IdempotencyTestDbContext>();
            var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);

            await cache.SetAsync("expired", [1], new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1) });
            await cache.SetAsync("live", [2], new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) });
            await cache.SetAsync("no-expiry", [3], new DistributedCacheEntryOptions());
            await Task.Delay(20);
        }

        await using var sweepProvider = BuildProvider(databaseName);
        var service = new IdempotencyCacheCleanupService<IdempotencyTestDbContext>(
            sweepProvider.GetRequiredService<IServiceScopeFactory>(),
            new IdempotencyCacheOptions { CleanupInterval = TimeSpan.FromMilliseconds(10) });

        await service.StartAsync(CancellationToken.None);
        await Task.Delay(50);
        await service.StopAsync(CancellationToken.None);

        await using var verifyContext = sweepProvider.GetRequiredService<IdempotencyTestDbContext>();
        var remainingKeys = await verifyContext.Set<IdempotencyCacheEntry>().Select(e => e.Key).ToListAsync();
        remainingKeys.Should().BeEquivalentTo("live", "no-expiry");
    }
}
