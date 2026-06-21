using FluentAssertions;
using Mediarq.Caching;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Idempotency;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mediarq.Idempotency.Tests;

public class IdempotencyBehaviorTests
{
    public record PayCommand(string Key, decimal Amount) : ICommand<Result<string>>, IIdempotentRequest
    {
        public string IdempotencyKey => Key;
    }

    public record PlainCommand : ICommand<Result<string>>;

    private static IDistributedCache NewCache()
        => new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

    private static IdempotencyBehavior<PayCommand, Result<string>> Behavior(IDistributedCache cache)
        => new(cache, new JsonMediarqCacheSerializer());

    [Fact]
    public async Task Replays_Stored_Result_And_Runs_Handler_Once_For_Same_Key()
    {
        var cache = NewCache();
        var behavior = Behavior(cache);

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Success($"receipt-{calls}"));
        }

        var context = new RequestContext<PayCommand, Result<string>>(new PayCommand("order-1", 10m), "user");

        var first = await behavior.Handle(context, Handle);
        var second = await behavior.Handle(context, Handle);

        calls.Should().Be(1);
        first.Value.Should().Be("receipt-1");
        second.Value.Should().Be("receipt-1");
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
            return Task.FromResult(Result.Success($"receipt-{calls}"));
        }

        await behavior.Handle(new RequestContext<PayCommand, Result<string>>(new PayCommand("a", 1m), "user"), Handle);
        await behavior.Handle(new RequestContext<PayCommand, Result<string>>(new PayCommand("b", 1m), "user"), Handle);

        calls.Should().Be(2);
    }

    [Fact]
    public void IsActive_True_For_Idempotent_False_Otherwise()
    {
        var cache = NewCache();

        new IdempotencyBehavior<PayCommand, Result<string>>(cache, new JsonMediarqCacheSerializer())
            .IsActive.Should().BeTrue();
        new IdempotencyBehavior<PlainCommand, Result<string>>(cache, new JsonMediarqCacheSerializer())
            .IsActive.Should().BeFalse();
    }

    [Fact]
    public void AddMediarqIdempotency_Registers_Behavior_And_Serializer()
    {
        var services = new ServiceCollection();

        services.AddMediarqIdempotency();

        services.Should().Contain(d => d.ServiceType == typeof(IMediarqCacheSerializer)
            && d.ImplementationType == typeof(JsonMediarqCacheSerializer));
        services.Should().Contain(d => d.ServiceType == typeof(IPipelineBehavior<,>)
            && d.ImplementationType == typeof(IdempotencyBehavior<,>));
    }
}
