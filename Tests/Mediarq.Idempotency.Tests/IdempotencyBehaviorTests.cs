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

    public record TicketCommand(string Key, TimeSpan? Retention) : ICommand<Result<string>>, IIdempotentRequest
    {
        public string IdempotencyKey => Key;

        public TimeSpan? IdempotencyDuration => Retention;
    }

    public record NullableCommand(string Key) : ICommand<string?>, IIdempotentRequest
    {
        public string IdempotencyKey => Key;
    }

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

    public record RefundCommand(string Key) : ICommand<Result<string>>, IIdempotentRequest
    {
        public string IdempotencyKey => Key;
    }

    [Fact]
    public async Task Does_Not_Replay_Across_Different_Request_Types_Sharing_A_Key()
    {
        var cache = NewCache();
        var pay = Behavior(cache);
        var refund = new IdempotencyBehavior<RefundCommand, Result<string>>(cache, new JsonMediarqCacheSerializer());

        var paid = await pay.Handle(
            new RequestContext<PayCommand, Result<string>>(new PayCommand("shared", 1m), "user"),
            () => Task.FromResult(Result.Success("payment")));
        var refunded = await refund.Handle(
            new RequestContext<RefundCommand, Result<string>>(new RefundCommand("shared"), "user"),
            () => Task.FromResult(Result.Success("refund")));

        paid.Value.Should().Be("payment");
        refunded.Value.Should().Be("refund");
    }

    [Fact]
    public async Task Does_Not_Store_A_Failed_Result_So_The_Same_Key_Can_Be_Retried()
    {
        var behavior = Behavior(NewCache());
        var context = new RequestContext<PayCommand, Result<string>>(new PayCommand("retry", 1m), "user");

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(calls == 1
                ? Result.Failure<string>(new ResultError("Pay.Declined", "declined", ErrorType.Validation))
                : Result.Success("ok"));
        }

        var first = await behavior.Handle(context, Handle);
        var second = await behavior.Handle(context, Handle);
        var third = await behavior.Handle(context, Handle);

        first.IsFailure.Should().BeTrue();
        second.IsSuccess.Should().BeTrue();
        third.Value.Should().Be("ok");
        calls.Should().Be(2);
    }

    [Fact]
    public async Task Concurrent_Calls_With_The_Same_Key_Run_The_Handler_Once()
    {
        var behavior = Behavior(NewCache());
        var calls = 0;

        async Task<Result<string>> Handle()
        {
            Interlocked.Increment(ref calls);
            await Task.Delay(50);
            return Result.Success("done");
        }

        var results = await Task.WhenAll(Enumerable.Range(0, 8).Select(_ =>
            behavior.Handle(new RequestContext<PayCommand, Result<string>>(new PayCommand("race", 1m), "user"), Handle)));

        calls.Should().Be(1);
        results.Should().OnlyContain(r => r.Value == "done");
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
    public async Task Does_Not_Store_A_Null_Response_So_The_Handler_Runs_Again()
    {
        var cache = NewCache();
        var behavior = new IdempotencyBehavior<NullableCommand, string?>(cache, new JsonMediarqCacheSerializer());

        var calls = 0;
        Task<string?> Handle()
        {
            calls++;
            // First call yields null (must not be stored); later calls yield a value.
            return Task.FromResult<string?>(calls == 1 ? null : $"value-{calls}");
        }

        var first = await behavior.Handle(new RequestContext<NullableCommand, string?>(new NullableCommand("k"), "user"), Handle);
        var second = await behavior.Handle(new RequestContext<NullableCommand, string?>(new NullableCommand("k"), "user"), Handle);

        first.Should().BeNull();
        second.Should().Be("value-2");
        calls.Should().Be(2);
    }

    [Fact]
    public async Task Honors_A_Custom_IdempotencyDuration_And_Still_Replays()
    {
        var cache = NewCache();
        var behavior = new IdempotencyBehavior<TicketCommand, Result<string>>(cache, new JsonMediarqCacheSerializer());

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Success($"ticket-{calls}"));
        }

        var request = new TicketCommand("t-1", TimeSpan.FromMinutes(5));
        var first = await behavior.Handle(new RequestContext<TicketCommand, Result<string>>(request, "user"), Handle);
        var second = await behavior.Handle(new RequestContext<TicketCommand, Result<string>>(request, "user"), Handle);

        calls.Should().Be(1);
        first.Value.Should().Be("ticket-1");
        second.Value.Should().Be("ticket-1");
    }

    [Fact]
    public void Constructor_Rejects_Null_Dependencies()
    {
        var cache = NewCache();

        var nullCache = () => new IdempotencyBehavior<PayCommand, Result<string>>(null!, new JsonMediarqCacheSerializer());
        var nullSerializer = () => new IdempotencyBehavior<PayCommand, Result<string>>(cache, null!);

        nullCache.Should().Throw<ArgumentNullException>().WithParameterName("cache");
        nullSerializer.Should().Throw<ArgumentNullException>().WithParameterName("serializer");
    }

    [Fact]
    public async Task Handle_Rejects_Null_Arguments()
    {
        var behavior = Behavior(NewCache());
        var context = new RequestContext<PayCommand, Result<string>>(new PayCommand("k", 1m), "user");

        var nullContext = async () => await behavior.Handle(null!, () => Task.FromResult(Result.Success("x")));
        var nullHandle = async () => await behavior.Handle(context, null!);

        await nullContext.Should().ThrowAsync<ArgumentNullException>().WithParameterName("context");
        await nullHandle.Should().ThrowAsync<ArgumentNullException>().WithParameterName("handle");
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

    [Fact]
    public void AddMediarqIdempotency_Rejects_A_Null_ServiceCollection()
    {
        var act = () => ((IServiceCollection)null!).AddMediarqIdempotency();

        act.Should().Throw<ArgumentNullException>().WithParameterName("services");
    }

    [Fact]
    public void AddMediarqIdempotency_Does_Not_Override_A_Custom_Serializer()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMediarqCacheSerializer, JsonMediarqCacheSerializer>();

        services.AddMediarqIdempotency();

        services.Where(d => d.ServiceType == typeof(IMediarqCacheSerializer)).Should().ContainSingle();
    }
}
