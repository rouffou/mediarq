using FluentAssertions;
using Mediarq.Core.Common.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Authorization.Tests;

public class AuthorizationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMediarqAuthorization_Registers_The_Behavior_And_Returns_Same_Instance()
    {
        var services = new ServiceCollection();

        var returned = services.AddMediarqAuthorization();

        returned.Should().BeSameAs(services);
        services.Any(d => d.ServiceType == typeof(IPipelineBehavior<,>) && d.ImplementationType == typeof(AuthorizationBehavior<,>))
            .Should().BeTrue();
    }

    [Fact]
    public void AddMediarqAuthorization_Throws_On_Null_Services()
    {
        IServiceCollection services = null!;

        ((Action)(() => services.AddMediarqAuthorization())).Should().Throw<ArgumentNullException>();
    }
}
