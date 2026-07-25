using FluentAssertions;
using Mediarq.HealthChecks.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mediarq.HealthChecks.Tests;

public class MediarqHandlerRegistrationHealthCheckTests
{
    private static readonly System.Reflection.Assembly TestAssembly = typeof(PingCommand).Assembly;

    private static HealthCheckContext Context(HealthStatus failureStatus = HealthStatus.Unhealthy) => new()
    {
        Registration = new HealthCheckRegistration("mediarq_handlers", _ => null!, failureStatus, tags: null)
    };

    [Fact]
    public async Task CheckHealthAsync_returns_healthy_when_every_request_has_exactly_one_handler()
    {
        var services = new ServiceCollection();
        Wiring.RegisterPingHandler(services);
        Wiring.RegisterCountHandler(services);
        var provider = services.BuildServiceProvider();

        var check = new MediarqHandlerRegistrationHealthCheck(provider, [TestAssembly]);
        var result = await check.CheckHealthAsync(Context());

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_returns_the_registration_failure_status_when_a_handler_is_missing()
    {
        var services = new ServiceCollection();
        Wiring.RegisterCountHandler(services); // PingCommand intentionally left unregistered
        var provider = services.BuildServiceProvider();

        var check = new MediarqHandlerRegistrationHealthCheck(provider, [TestAssembly]);
        var result = await check.CheckHealthAsync(Context(HealthStatus.Degraded));

        result.Status.Should().Be(HealthStatus.Degraded);
        result.Data.Should().ContainKey(typeof(PingCommand).FullName!);
    }
}
