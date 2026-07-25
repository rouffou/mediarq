using System.Threading.RateLimiting;
using FluentAssertions;

namespace Mediarq.RateLimiting.Tests;

public class RateLimiterRegistryTests
{
    private static Func<string, RateLimitPartition<string>> NoLimitPartitioner =>
        key => RateLimitPartition.GetNoLimiter(key);

    [Fact]
    public void GetPolicy_Throws_For_An_Unregistered_Name()
    {
        var registry = new RateLimiterRegistry();

        var act = () => registry.GetPolicy("missing");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetPolicy_Returns_The_Registered_Limiter()
    {
        var registry = new RateLimiterRegistry();
        registry.AddPolicy("p", NoLimitPartitioner);

        var act = () => registry.GetPolicy("p");

        act.Should().NotThrow();
    }

    [Fact]
    public void AddPolicy_Throws_When_The_Same_Name_Is_Registered_Twice()
    {
        var registry = new RateLimiterRegistry();
        registry.AddPolicy("dup", NoLimitPartitioner);

        var act = () => registry.AddPolicy("dup", NoLimitPartitioner);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AddPolicy_Throws_On_Invalid_Name(string? name)
    {
        var registry = new RateLimiterRegistry();

        var act = () => registry.AddPolicy(name!, NoLimitPartitioner);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddPolicy_Throws_On_Null_Partitioner()
    {
        var registry = new RateLimiterRegistry();

        var act = () => registry.AddPolicy("p", null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
