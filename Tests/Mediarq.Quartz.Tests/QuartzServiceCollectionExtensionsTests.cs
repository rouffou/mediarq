using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Quartz.Tests;

public class QuartzServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMediarqQuartz_Registers_The_Job_Type()
    {
        var services = new ServiceCollection();

        services.AddMediarqQuartz();
        var provider = services.BuildServiceProvider();

        // The job depends on ISender, which isn't registered here; resolving it should fail with a DI
        // error about the missing dependency, not "service not found" -- proving the job type itself was
        // registered (TryAddTransient), just its dependency graph is incomplete in this minimal setup.
        var act = () => provider.GetRequiredService<MediarqQuartzJob>();
        act.Should().Throw<InvalidOperationException>().WithMessage("*ISender*");
    }

    [Fact]
    public void AddMediarqQuartz_Throws_For_A_Null_Services_Collection()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqQuartz();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqQuartz_Returns_The_Same_Service_Collection()
    {
        var services = new ServiceCollection();

        var result = services.AddMediarqQuartz();

        result.Should().BeSameAs(services);
    }
}
