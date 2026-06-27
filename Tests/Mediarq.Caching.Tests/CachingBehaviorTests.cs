using FluentAssertions;
using Mediarq.Caching;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Caching.Tests;

public class CachingBehaviorTests
{
    public record CachedQuery(string Key) : IQuery<Result<string>>, ICacheableRequest
    {
        public string CacheKey => Key;
    }

    public record PlainQuery : IQuery<Result<string>>;

    private static CachingBehavior<TRequest, TResponse> Behavior<TRequest, TResponse>(IMemoryCache cache)
        where TRequest : Core.Common.Requests.Abstraction.ICommandOrQuery<TResponse>
        => new(cache);

    [Fact]
    public async Task Caches_Response_And_Skips_Handler_On_Hit()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var behavior = Behavior<CachedQuery, Result<string>>(cache);
        var request = new CachedQuery("k1");
        var context = new RequestContext<CachedQuery, Result<string>>(request, "user");

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Success("value"));
        }

        var first = await behavior.Handle(context, Handle);
        var second = await behavior.Handle(context, Handle);

        calls.Should().Be(1);
        first.Value.Should().Be("value");
        second.Value.Should().Be("value");
    }

    [Fact]
    public void IsActive_True_For_Cacheable_False_Otherwise()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());

        // The executor only runs the behavior for cacheable request types; non-cacheable requests are
        // skipped entirely (so the behavior never sees them).
        Behavior<CachedQuery, Result<string>>(cache).IsActive.Should().BeTrue();
        Behavior<PlainQuery, Result<string>>(cache).IsActive.Should().BeFalse();
    }

    [Fact]
    public void AddMediarqCaching_Registers_Behavior_And_MemoryCache()
    {
        var services = new ServiceCollection();

        services.AddMediarqCaching();

        services.Should().Contain(d => d.ServiceType == typeof(IMemoryCache));
        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(CachingBehavior<,>));
    }
}
