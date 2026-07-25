using FluentAssertions;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Mediarq.Testing.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Testing.Tests;

public class SpyMediatorServiceCollectionExtensionsTests
{
    [Fact]
    public async Task AddMediarqSpy_Decorates_IMediator_And_Records_Real_Dispatches()
    {
        var handledIds = new List<int>();
        var services = new ServiceCollection();
        services.AddSingleton(handledIds);
        services.AddMediarq(isHttp: false, typeof(SpyMediatorServiceCollectionExtensionsTests).Assembly);
        services.AddMediarqSpy();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        mediator.Should().BeOfType<SpyMediator>();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var response = await sender.Send<Result<string>>(new PingCommand("hi"));
        await publisher.Publish(new Pinged(42));

        response.Value.Should().Be("hi"); // the real handler ran, not a fake
        handledIds.Should().ContainSingle().Which.Should().Be(42); // the real notification handler ran

        var spy = (SpyMediator)mediator;
        spy.HasSent<PingCommand>().Should().BeTrue();
        spy.HasPublished<Pinged>().Should().BeTrue();
    }

    [Fact]
    public void AddMediarqSpy_Throws_On_Null_Services()
    {
        IServiceCollection services = null!;

        ((Action)(() => services.AddMediarqSpy())).Should().Throw<ArgumentNullException>();
    }
}
