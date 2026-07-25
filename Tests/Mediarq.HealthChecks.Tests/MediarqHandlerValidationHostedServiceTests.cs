using FluentAssertions;
using Mediarq.HealthChecks.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.HealthChecks.Tests;

public class MediarqHandlerValidationHostedServiceTests
{
    private static readonly System.Reflection.Assembly TestAssembly = typeof(PingCommand).Assembly;

    [Fact]
    public async Task AddMediarqHandlerValidationOnStartup_does_not_throw_when_registrations_are_valid()
    {
        var services = new ServiceCollection();
        Wiring.RegisterPingHandler(services);
        Wiring.RegisterCountHandler(services);
        services.AddMediarqHandlerValidationOnStartup(TestAssembly);
        var provider = services.BuildServiceProvider();

        var hostedService = provider.GetServices<Microsoft.Extensions.Hosting.IHostedService>().Single();
        var act = () => hostedService.StartAsync(CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task AddMediarqHandlerValidationOnStartup_throws_when_a_handler_is_missing()
    {
        var services = new ServiceCollection();
        Wiring.RegisterCountHandler(services); // PingCommand intentionally left unregistered
        services.AddMediarqHandlerValidationOnStartup(TestAssembly);
        var provider = services.BuildServiceProvider();

        var hostedService = provider.GetServices<Microsoft.Extensions.Hosting.IHostedService>().Single();
        var act = () => hostedService.StartAsync(CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .WithMessage("*PingCommand*");
    }
}
