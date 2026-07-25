using System.Threading.RateLimiting;
using FluentAssertions;
using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.RateLimiting.Tests;

public class RateLimitingServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMediarqRateLimiting_Registers_The_Registry_And_The_Behavior()
    {
        var services = new ServiceCollection();

        var returned = services.AddMediarqRateLimiting(registry =>
            registry.AddPolicy("p", key => RateLimitPartition.GetNoLimiter(key)));

        returned.Should().BeSameAs(services);
        services.Should().Contain(d => d.ServiceType == typeof(RateLimiterRegistry) && d.Lifetime == ServiceLifetime.Singleton);
        services.Should().Contain(d =>
            d.ServiceType == typeof(IPipelineBehavior<,>) && d.ImplementationType == typeof(RateLimitingBehavior<,>));
    }

    [Fact]
    public void AddMediarqRateLimiting_Invokes_Configure_Immediately()
    {
        var services = new ServiceCollection();
        var configured = false;

        services.AddMediarqRateLimiting(_ => configured = true);

        configured.Should().BeTrue();
    }

    [Fact]
    public void AddMediarqRateLimiting_Throws_On_Null_Services()
    {
        IServiceCollection services = null!;

        ((Action)(() => services.AddMediarqRateLimiting(_ => { }))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqRateLimiting_Throws_On_Null_Configure()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMediarqRateLimiting(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
