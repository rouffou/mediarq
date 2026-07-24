using FluentAssertions;
using Mediarq.Saga;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Saga.Tests;

public class SagaServiceCollectionExtensionsTests
{
    private sealed class State : ISagaState
    {
        public required Guid CorrelationId { get; init; }
        public bool IsComplete { get; set; }
    }

    private sealed class CustomStore : ISagaStore<State>
    {
        public Task<State?> FindAsync(Guid correlationId, CancellationToken cancellationToken = default)
            => Task.FromResult<State?>(null);

        public Task SaveAsync(State state, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Guid correlationId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    [Fact]
    public void AddMediarqSaga_Registers_The_In_Memory_Store_By_Default()
    {
        var services = new ServiceCollection();

        services.AddMediarqSaga<State>();
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ISagaStore<State>>().Should().BeOfType<InMemorySagaStore<State>>();
    }

    [Fact]
    public void AddMediarqSaga_Does_Not_Override_An_Already_Registered_Store()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISagaStore<State>, CustomStore>();

        services.AddMediarqSaga<State>();
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ISagaStore<State>>().Should().BeOfType<CustomStore>();
    }

    [Fact]
    public void AddMediarqSaga_Throws_For_Null_Services()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqSaga<State>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqSaga_Returns_The_Same_Service_Collection()
    {
        var services = new ServiceCollection();

        var result = services.AddMediarqSaga<State>();

        result.Should().BeSameAs(services);
    }
}
