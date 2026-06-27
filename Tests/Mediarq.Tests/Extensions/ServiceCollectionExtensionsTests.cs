using FluentAssertions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Pipeline.Behaviors;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Tests.Extensions;

/// <summary>
/// Covers the opt-in registration helpers and argument guards of
/// <see cref="ServiceCollectionExtensions"/> not exercised by the end-to-end pipeline tests.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    private static bool RegistersBehavior(IServiceCollection services, Type behavior) =>
        services.Any(d =>
            d.ServiceType == typeof(IPipelineBehavior<,>) && d.ImplementationType == behavior);

    [Fact]
    public void AddMediarqRequestLogging_Registers_LoggingBehavior_And_Returns_Same_Instance()
    {
        var services = new ServiceCollection();

        var returned = services.AddMediarqRequestLogging();

        returned.Should().BeSameAs(services);
        RegistersBehavior(services, typeof(LoggingBehavior<,>)).Should().BeTrue();
    }

    [Fact]
    public void AddMediarqPerformanceTracking_Registers_PerformanceBehavior()
    {
        var services = new ServiceCollection();

        services.AddMediarqPerformanceTracking();

        RegistersBehavior(services, typeof(PerformanceBehavior<,>)).Should().BeTrue();
    }

    [Fact]
    public void AddMediarqTimeout_Registers_TimeoutBehavior()
    {
        var services = new ServiceCollection();

        services.AddMediarqTimeout();

        RegistersBehavior(services, typeof(TimeoutBehavior<,>)).Should().BeTrue();
    }

    [Fact]
    public void AddMediarqCore_Registers_The_Builtin_Plumbing_Behaviors()
    {
        var services = new ServiceCollection();

        services.AddMediarqCore();

        RegistersBehavior(services, typeof(ValidationBehavior<,>)).Should().BeTrue();
        RegistersBehavior(services, typeof(RequestPreProcessorBehavior<,>)).Should().BeTrue();
        RegistersBehavior(services, typeof(RequestPostProcessorBehavior<,>)).Should().BeTrue();
        RegistersBehavior(services, typeof(RequestExceptionProcessorBehavior<,>)).Should().BeTrue();
    }

    [Fact]
    public void AddMediarq_Throws_When_An_Assembly_In_The_Array_Is_Null()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMediarq(isHttp: false, new System.Reflection.Assembly[] { null! });

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Registration_Methods_Throw_On_Null_Services()
    {
        IServiceCollection services = null!;

        ((Action)(() => services.AddMediarqCore())).Should().Throw<ArgumentNullException>();
        ((Action)(() => services.AddMediarqRequestLogging())).Should().Throw<ArgumentNullException>();
        ((Action)(() => services.AddMediarqPerformanceTracking())).Should().Throw<ArgumentNullException>();
        ((Action)(() => services.AddMediarqTimeout())).Should().Throw<ArgumentNullException>();
        ((Action)(() => services.AddMediarq())).Should().Throw<ArgumentNullException>();
    }
}
