using FluentAssertions;
using Mediarq.HealthChecks.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.HealthChecks.Tests;

public class MediarqHandlerRegistrationValidatorTests
{
    private static readonly System.Reflection.Assembly TestAssembly = typeof(PingCommand).Assembly;

    [Fact]
    public void Validate_reports_no_issue_for_a_request_type_with_exactly_one_handler()
    {
        var services = new ServiceCollection();
        Wiring.RegisterPingHandler(services);
        Wiring.RegisterCountHandler(services);
        var provider = services.BuildServiceProvider();

        var issues = MediarqHandlerRegistrationValidator.Validate(provider, TestAssembly);

        issues.Should().NotContain(i => i.RequestType == typeof(PingCommand));
        issues.Should().NotContain(i => i.RequestType == typeof(CountQuery));
    }

    [Fact]
    public void Validate_reports_a_zero_count_issue_when_a_handler_is_missing()
    {
        var services = new ServiceCollection();
        Wiring.RegisterCountHandler(services); // PingCommand intentionally left unregistered
        var provider = services.BuildServiceProvider();

        var issues = MediarqHandlerRegistrationValidator.Validate(provider, TestAssembly);

        issues.Should().ContainSingle(i => i.RequestType == typeof(PingCommand) && i.HandlerCount == 0);
    }

    [Fact]
    public void Validate_reports_the_handler_count_when_more_than_one_is_registered()
    {
        var services = new ServiceCollection();
        Wiring.RegisterPingHandler(services);
        services.AddScoped<Mediarq.Core.Common.Requests.Abstraction.IRequestHandler<PingCommand, Mediarq.Core.Common.Requests.Abstraction.Unit>, PingCommandExtraHandler>();
        Wiring.RegisterCountHandler(services);
        var provider = services.BuildServiceProvider();

        var issues = MediarqHandlerRegistrationValidator.Validate(provider, TestAssembly);

        issues.Should().ContainSingle(i => i.RequestType == typeof(PingCommand) && i.HandlerCount == 2);
    }

    [Fact]
    public void Validate_throws_when_serviceProvider_is_null()
    {
        var act = () => MediarqHandlerRegistrationValidator.Validate(null!, TestAssembly);

        act.Should().Throw<ArgumentNullException>();
    }
}
