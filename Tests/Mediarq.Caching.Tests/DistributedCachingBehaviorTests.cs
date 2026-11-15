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

    public record TimedQuery(string Key, TimeSpan? Duration) : IQuery<Result<string>>, ICacheableRequest
    {
        public string CacheKey => Key;

        public TimeSpan? CacheDuration => Duration;
    }

    public record NullableQuery(string Key) : IQuery<string?>, ICacheableRequest
    {
        public string CacheKey => Key;
    }

    public record PlainQuery : IQuery<Result<string>>;

    public sealed record OrderDto(int Id, string Customer, decimal Total);

    public record FailureCachingQuery(string Key) : IQuery<Result<string>>, ICacheableRequest
    {
        public string CacheKey => Key;

        public bool CacheFailures => true;
    }

    private static IDistributedCache NewCache()
        => new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

    [Fact]
    public async Task Does_Not_Cache_A_Failed_Result_By_Default()
    {
        var behavior = Behavior(NewCache());
        var context = new RequestContext<CachedQuery, Result<string>>(new CachedQuery("fail"), "user");

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(calls == 1
                ? Result.Failure<string>(new ResultError("Q.Transient", "transient", ErrorType.Validation))
                : Result.Success("ok"));
        }

        var first = await behavior.Handle(context, Handle);
        var second = await behavior.Handle(context, Handle);
        var third = await behavior.Handle(context, Handle);

        first.IsFailure.Should().BeTrue();
        second.Value.Should().Be("ok");
        third.Value.Should().Be("ok");
        calls.Should().Be(2);
    }

    [Fact]
    public async Task Caches_A_Failed_Result_When_The_Request_Opts_In()
    {
        var behavior = new DistributedCachingBehavior<FailureCachingQuery, Result<string>>(NewCache(), new JsonMediarqCacheSerializer());
        var context = new RequestContext<FailureCachingQuery, Result<string>>(new FailureCachingQuery("fail"), "user");

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Failure<string>(new ResultError("Q.Permanent", "permanent", ErrorType.Validation)));
        }

        await behavior.Handle(context, Handle);
        var second = await behavior.Handle(context, Handle);

        second.IsFailure.Should().BeTrue();
        calls.Should().Be(1);
    }

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
    public async Task Runs_Handler_Again_For_A_Different_Key()
    {
        var cache = NewCache();
        var behavior = Behavior(cache);

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
    public async Task Does_Not_Cache_A_Null_Response_So_The_Handler_Runs_Again()
    {
        var cache = NewCache();
        var behavior = new DistributedCachingBehavior<NullableQuery, string?>(cache, new JsonMediarqCacheSerializer());

        var calls = 0;
        Task<string?> Handle()
        {
            calls++;
            return Task.FromResult<string?>(calls == 1 ? null : $"value-{calls}");
        }

        var first = await behavior.Handle(new RequestContext<NullableQuery, string?>(new NullableQuery("k"), "user"), Handle);
        var second = await behavior.Handle(new RequestContext<NullableQuery, string?>(new NullableQuery("k"), "user"), Handle);

        first.Should().BeNull();
        second.Should().Be("value-2");
        calls.Should().Be(2);
    }

    [Fact]
    public async Task Honors_A_Custom_CacheDuration_And_Still_Serves_From_Cache()
    {
        var cache = NewCache();
        var behavior = new DistributedCachingBehavior<TimedQuery, Result<string>>(cache, new JsonMediarqCacheSerializer());

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
    public void Constructor_Rejects_Null_Dependencies()
    {
        var cache = NewCache();

        var nullCache = () => new DistributedCachingBehavior<CachedQuery, Result<string>>(null!, new JsonMediarqCacheSerializer());
        var nullSerializer = () => new DistributedCachingBehavior<CachedQuery, Result<string>>(cache, null!);

        nullCache.Should().Throw<ArgumentNullException>().WithParameterName("cache");
        nullSerializer.Should().Throw<ArgumentNullException>().WithParameterName("serializer");
    }

    [Fact]
    public async Task Handle_Rejects_Null_Arguments()
    {
        var behavior = Behavior(NewCache());
        var context = new RequestContext<CachedQuery, Result<string>>(new CachedQuery("k"), "user");

        var nullContext = async () => await behavior.Handle(null!, () => Task.FromResult(Result.Success("x")));
        var nullHandle = async () => await behavior.Handle(context, null!);

        await nullContext.Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        await nullHandle.Should().ThrowAsync<ArgumentNullException>().WithParameterName("handle");
    }

    [Fact]
    public void AddMediarqDistributedCaching_Rejects_A_Null_ServiceCollection()
    {
        var act = () => ((IServiceCollection)null!).AddMediarqDistributedCaching();

        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    [Fact]
    public void AddMediarqDistributedCaching_Does_Not_Override_A_Custom_Serializer()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMediarqCacheSerializer, JsonMediarqCacheSerializer>();

        services.AddMediarqDistributedCaching();

        services.Where(d => d.ServiceType == typeof(IMediarqCacheSerializer)).Should().ContainSingle();
    }

    [Fact]
    public void Json_Serializer_Round_Trips()
    {
        var serializer = new JsonMediarqCacheSerializer();

        var bytes = serializer.Serialize("hello");
        serializer.Deserialize<string>(bytes).Should().Be("hello");
    }

    [Fact]
    public void Json_Serializer_Round_Trips_A_Result()
    {
        var serializer = new JsonMediarqCacheSerializer();

        var bytes = serializer.Serialize(Result.Success("receipt"));
        var restored = serializer.Deserialize<Result<string>>(bytes);

        restored.Should().NotBeNull();
        restored!.IsSuccess.Should().BeTrue();
        restored.Value.Should().Be("receipt");
    }

    [Fact]
    public void Json_Serializer_Round_Trips_A_Complex_Dto()
    {
        var serializer = new JsonMediarqCacheSerializer();

        var bytes = serializer.Serialize(new OrderDto(7, "Ada", 19.99m));
        serializer.Deserialize<OrderDto>(bytes).Should().Be(new OrderDto(7, "Ada", 19.99m));
    }
}
