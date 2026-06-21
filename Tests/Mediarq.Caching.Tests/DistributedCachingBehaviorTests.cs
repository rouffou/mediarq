using FluentAssertions;
using Mediarq.Caching;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mediarq.Caching.Tests;

public class DistributedCachingBehaviorTests
{
    public record CachedQuery(string Key) : IQuery<Result<string>>, ICacheableRequest
    {
        public string CacheKey => Key;
    }

    public record PlainQuery : IQuery<Result<string>>;

    private static IDistributedCache NewCache()
        => new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

    private static DistributedCachingBehavior<CachedQuery, Result<string>> Behavior(IDistributedCache cache)
        => new(cache, new JsonMediarqCacheSerializer());

    [Fact]
    public async Task Caches_Response_And_Skips_Handler_On_Hit()
    {
        var cache = NewCache();
        var behavior = Behavior(cache);
        var context = new RequestContext<CachedQuery, Result<string>>(new CachedQuery("k1"), "user");

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
        var cache = NewCache();

        new DistributedCachingBehavior<CachedQuery, Result<string>>(cache, new JsonMediarqCacheSerializer())
            .IsActive.Should().BeTrue();
        new DistributedCachingBehavior<PlainQuery, Result<string>>(cache, new JsonMediarqCacheSerializer())
            .IsActive.Should().BeFalse();
    }

    [Fact]
    public void AddMediarqDistributedCaching_Registers_Behavior_And_Serializer()
    {
        var services = new ServiceCollection();

        services.AddMediarqDistributedCaching();

        services.Should().Contain(d => d.ServiceType == typeof(IMediarqCacheSerializer)
            && d.ImplementationType == typeof(JsonMediarqCacheSerializer));
        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(DistributedCachingBehavior<,>));
    }

    [Fact]
    public void Json_Serializer_Round_Trips()
    {
        var serializer = new JsonMediarqCacheSerializer();

        var bytes = serializer.Serialize("hello");
        serializer.Deserialize<string>(bytes).Should().Be("hello");
    }
}
