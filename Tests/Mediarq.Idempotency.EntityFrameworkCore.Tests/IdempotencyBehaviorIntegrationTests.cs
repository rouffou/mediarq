using FluentAssertions;
using Mediarq.Caching;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace Mediarq.Idempotency.EntityFrameworkCore.Tests;

/// <summary>
/// End-to-end check that <see cref="IdempotencyBehavior{TRequest, TResponse}"/> — unchanged, still
/// depending on the plain <c>IDistributedCache</c> abstraction — works against
/// <see cref="EfCoreDistributedCache{TContext}"/> exactly like it does against any other implementation.
/// </summary>
public class IdempotencyBehaviorIntegrationTests
{
    public record PayCommand(string Key, decimal Amount) : ICommand<Result<string>>, IIdempotentRequest
    {
        public string IdempotencyKey => Key;
    }

    private static IdempotencyTestDbContext NewContext()
        => new(new DbContextOptionsBuilder<IdempotencyTestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Replays_Stored_Result_And_Runs_Handler_Once_For_Same_Key()
    {
        await using var context = NewContext();
        var cache = new EfCoreDistributedCache<IdempotencyTestDbContext>(context);
        var behavior = new IdempotencyBehavior<PayCommand, Result<string>>(cache, new JsonMediarqCacheSerializer());

        var calls = 0;
        Task<Result<string>> Handle()
        {
            calls++;
            return Task.FromResult(Result.Success($"receipt-{calls}"));
        }

        var requestContext = new RequestContext<PayCommand, Result<string>>(new PayCommand("order-1", 10m), "user");

        var first = await behavior.Handle(requestContext, Handle);
        var second = await behavior.Handle(requestContext, Handle);

        calls.Should().Be(1);
        first.Value.Should().Be("receipt-1");
        second.Value.Should().Be("receipt-1");

        // The result really did round-trip through the database, not an in-process cache.
        (await context.Set<IdempotencyCacheEntry>().CountAsync()).Should().Be(1);
    }
}
