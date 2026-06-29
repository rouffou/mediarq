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

    public record TimedQuery(string Key, TimeSpan? Duration) : IQuery<Result<string>>, ICacheableRequest
    {
        public string CacheKey => Key;

        public TimeSpan? CacheDuration => Duration;
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
    public async Task Runs_Handler_Again_For_A_Different_Key()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var behavior = Behavior<CachedQuery, Result<string>>(cache);

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Success($"value-{calls}"));
        }

        await behavior.Handle(new RequestContext<CachedQuery, Result<string>>(new CachedQuery("a"), "user"), Handle);
        await behavior.Handle(new RequestContext<CachedQuery, Result<string>>(new CachedQuery("b"), "user"), Handle);

        calls.Should().Be(2);
    }

    [Fact]
    public async Task Honors_A_Custom_CacheDuration_And_Still_Serves_From_Cache()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var behavior = Behavior<TimedQuery, Result<string>>(cache);

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Success($"value-{calls}"));
        }

        var request = new TimedQuery("k", TimeSpan.FromMinutes(5));
        await behavior.Handle(new RequestContext<TimedQuery, Result<string>>(request, "user"), Handle);
        var second = await behavior.Handle(new RequestContext<TimedQuery, Result<string>>(request, "user"), Handle);

        calls.Should().Be(1);
        second.Value.Should().Be("value-1");
    }

    [Fact]
    public void Constructor_Rejects_A_Null_Cache()
    {
        var act = () => new CachingBehavior<CachedQuery, Result<string>>(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("cache");
    }

    [Fact]
    public async Task Handle_Rejects_Null_Arguments()
    {
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var behavior = Behavior<CachedQuery, Result<string>>(cache);
        var context = new RequestContext<CachedQuery, Result<string>>(new CachedQuery("k"), "user");

        var nullContext = async () => await behavior.Handle(null!, () => Task.FromResult(Result.Success("x")));
        var nullHandle = async () => await behavior.Handle(context, null!);

        await nullContext.Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        await nullHandle.Should().ThrowAsync<ArgumentNullException>().WithParameterName("handle");
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

    [Fact]
    public void AddMediarqCaching_Rejects_A_Null_ServiceCollection()
    {
        var act = () => ((IServiceCollection)null!).AddMediarqCaching();

        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }
}
