using FluentAssertions;
using Mediarq.Saga;

namespace Mediarq.Saga.Tests;

public class InMemorySagaStoreTests
{
    private sealed class State : ISagaState
    {
        public required Guid CorrelationId { get; init; }
        public bool IsComplete { get; set; }
        public int Step { get; set; }
    }

    [Fact]
    public async Task FindAsync_Returns_Null_For_Unknown_Correlation_Id()
    {
        var store = new InMemorySagaStore<State>();

        var found = await store.FindAsync(Guid.NewGuid());

        found.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_Then_FindAsync_Round_Trips_The_Same_State()
    {
        var store = new InMemorySagaStore<State>();
        var correlationId = Guid.NewGuid();
        var state = new State { CorrelationId = correlationId, Step = 3 };

        await store.SaveAsync(state);
        var found = await store.FindAsync(correlationId);

        found.Should().NotBeNull();
        found!.Step.Should().Be(3);
    }

    [Fact]
    public async Task SaveAsync_Overwrites_The_Existing_Instance_For_The_Same_Correlation_Id()
    {
        var store = new InMemorySagaStore<State>();
        var correlationId = Guid.NewGuid();

        await store.SaveAsync(new State { CorrelationId = correlationId, Step = 1 });
        await store.SaveAsync(new State { CorrelationId = correlationId, Step = 2 });
        var found = await store.FindAsync(correlationId);

        found!.Step.Should().Be(2);
    }

    [Fact]
    public async Task DeleteAsync_Removes_The_Instance()
    {
        var store = new InMemorySagaStore<State>();
        var correlationId = Guid.NewGuid();
        await store.SaveAsync(new State { CorrelationId = correlationId });

        await store.DeleteAsync(correlationId);
        var found = await store.FindAsync(correlationId);

        found.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_On_An_Unknown_Correlation_Id_Does_Not_Throw()
    {
        var store = new InMemorySagaStore<State>();

        var act = () => store.DeleteAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SaveAsync_Throws_For_Null_State()
    {
        var store = new InMemorySagaStore<State>();

        var act = () => store.SaveAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
