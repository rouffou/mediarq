using FluentAssertions;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;

namespace Mediarq.Tests.Core.Requests.Abstraction;

/// <summary>
/// Exercises <see cref="IRequestHandler{TRequest}"/>'s default-interface adaptation to
/// <see cref="IRequestHandler{TRequest, Unit}"/> directly (bypassing the mediator) to pin down its
/// synchronous-completion fast path and its fallback for a genuinely asynchronous handler.
/// </summary>
public class RequestHandlerVoidAdapterTests
{
    private sealed record VoidCommand : ICommand;

    private sealed class SyncCompletingHandler : ICommandHandler<VoidCommand>
    {
        public Task Handle(VoidCommand request, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class AsyncCompletingHandler : ICommandHandler<VoidCommand>
    {
        public async Task Handle(VoidCommand request, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
        }
    }

    private sealed class SyncThrowingHandler : ICommandHandler<VoidCommand>
    {
        public Task Handle(VoidCommand request, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("sync boom");
    }

    private sealed class AsyncThrowingHandler : ICommandHandler<VoidCommand>
    {
        public async Task Handle(VoidCommand request, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            throw new InvalidOperationException("async boom");
        }
    }

    private static IRequestHandler<VoidCommand, Unit> Adapter(IRequestHandler<VoidCommand> handler) => handler;

    [Fact]
    public async Task Returns_Unit_Value_For_A_Synchronously_Completing_Handler()
    {
        var result = await Adapter(new SyncCompletingHandler()).Handle(new VoidCommand());

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Returns_Unit_Value_For_A_Genuinely_Asynchronous_Handler()
    {
        var result = await Adapter(new AsyncCompletingHandler()).Handle(new VoidCommand());

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Propagates_An_Exception_Thrown_Synchronously_By_The_Handler()
    {
        var act = () => Adapter(new SyncThrowingHandler()).Handle(new VoidCommand());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("sync boom");
    }

    [Fact]
    public async Task Propagates_An_Exception_Thrown_By_A_Genuinely_Asynchronous_Handler()
    {
        var act = () => Adapter(new AsyncThrowingHandler()).Handle(new VoidCommand());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("async boom");
    }
}
