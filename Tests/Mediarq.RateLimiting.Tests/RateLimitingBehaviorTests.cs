using System.Threading.RateLimiting;
using FluentAssertions;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Results;
using Mediarq.RateLimiting.Tests.Fixtures;

namespace Mediarq.RateLimiting.Tests;

public class RateLimitingBehaviorTests
{
    private static RateLimiterRegistry SingleTokenRegistry(string policyName)
    {
        var registry = new RateLimiterRegistry();
        registry.AddPolicy(policyName, key => RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
        {
            Window = TimeSpan.FromMinutes(1),
            PermitLimit = 1,
            QueueLimit = 0,
            AutoReplenishment = false,
        }));
        return registry;
    }

    [Fact]
    public void IsActive_Reflects_Whether_The_Request_Type_Implements_IRateLimitedRequest()
    {
        var registry = new RateLimiterRegistry();

        new RateLimitingBehavior<LimitedCommand, Result<string>>(registry).IsActive.Should().BeTrue();
        new RateLimitingBehavior<PlainCommand, Result<string>>(registry).IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Invokes_The_Handler_When_A_Permit_Is_Available()
    {
        var registry = SingleTokenRegistry("policy-a");
        var behavior = new RateLimitingBehavior<LimitedCommand, Result<string>>(registry);
        var request = new LimitedCommand("policy-a", "user-1");
        var context = new RequestContext<LimitedCommand, Result<string>>(request, "user");

        var result = await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_Throws_RateLimitExceededException_When_No_Permit_Is_Available()
    {
        var registry = SingleTokenRegistry("policy-b");
        var behavior = new RateLimitingBehavior<LimitedCommand, Result<string>>(registry);
        var request = new LimitedCommand("policy-b", "user-1");
        var context = new RequestContext<LimitedCommand, Result<string>>(request, "user");

        // First call consumes the single permit.
        await behavior.Handle(context, () => Task.FromResult(Result.Success("ok")));

        var handlerCalled = false;
        var act = async () => await behavior.Handle(context, () =>
        {
            handlerCalled = true;
            return Task.FromResult(Result.Success("should not run"));
        });

        var exception = await act.Should().ThrowAsync<RateLimitExceededException>();
        exception.Which.PolicyName.Should().Be("policy-b");
        exception.Which.PartitionKey.Should().Be("user-1");
        handlerCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Treats_Different_Partition_Keys_As_Independent_Limits()
    {
        var registry = SingleTokenRegistry("policy-c");
        var behavior = new RateLimitingBehavior<LimitedCommand, Result<string>>(registry);

        var contextUser1 = new RequestContext<LimitedCommand, Result<string>>(new LimitedCommand("policy-c", "user-1"), "user");
        var contextUser2 = new RequestContext<LimitedCommand, Result<string>>(new LimitedCommand("policy-c", "user-2"), "user");

        await behavior.Handle(contextUser1, () => Task.FromResult(Result.Success("user1-first")));

        // user-1's single permit is now exhausted, but user-2 is an independent partition.
        var user2Result = await behavior.Handle(contextUser2, () => Task.FromResult(Result.Success("user2-first")));
        user2Result.Value.Should().Be("user2-first");

        var act = async () => await behavior.Handle(contextUser1, () => Task.FromResult(Result.Success("should not run")));
        await act.Should().ThrowAsync<RateLimitExceededException>();
    }

    [Fact]
    public async Task Handle_Treats_A_Null_PartitionKey_As_A_Single_Shared_Bucket()
    {
        var registry = SingleTokenRegistry("policy-d");
        var behavior = new RateLimitingBehavior<LimitedCommand, Result<string>>(registry);

        var context1 = new RequestContext<LimitedCommand, Result<string>>(new LimitedCommand("policy-d", null), "user");
        var context2 = new RequestContext<LimitedCommand, Result<string>>(new LimitedCommand("policy-d", null), "user");

        await behavior.Handle(context1, () => Task.FromResult(Result.Success("first")));

        var act = async () => await behavior.Handle(context2, () => Task.FromResult(Result.Success("should not run")));

        (await act.Should().ThrowAsync<RateLimitExceededException>()).Which.PartitionKey.Should().Be("*");
    }

    [Fact]
    public async Task Handle_Throws_When_The_Policy_Is_Not_Registered()
    {
        var registry = new RateLimiterRegistry();
        var behavior = new RateLimitingBehavior<LimitedCommand, Result<string>>(registry);
        var context = new RequestContext<LimitedCommand, Result<string>>(new LimitedCommand("missing-policy", null), "user");

        var act = async () => await behavior.Handle(context, () => Task.FromResult(Result.Success("should not run")));

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_Throws_On_Null_Registry()
    {
        var act = () => new RateLimitingBehavior<LimitedCommand, Result<string>>(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task Handle_Rejects_Null_Arguments()
    {
        var behavior = new RateLimitingBehavior<LimitedCommand, Result<string>>(new RateLimiterRegistry());
        var context = new RequestContext<LimitedCommand, Result<string>>(new LimitedCommand("p", null), "user");

        var actNullContext = async () => await behavior.Handle(null!, () => Task.FromResult(Result.Success("x")));
        var actNullHandle = async () => await behavior.Handle(context, null!);

        await actNullContext.Should().ThrowAsync<ArgumentNullException>();
        await actNullHandle.Should().ThrowAsync<ArgumentNullException>();
    }
}
