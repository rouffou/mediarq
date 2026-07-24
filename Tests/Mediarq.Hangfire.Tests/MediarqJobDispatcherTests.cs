using FluentAssertions;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Mediators;

namespace Mediarq.Hangfire.Tests;

public class MediarqJobDispatcherTests
{
    private sealed record TestCommand(string Value) : ICommand;

    private sealed class RecordingSender : ISender
    {
        public object? LastCommand { get; private set; }

        public Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Only void commands are dispatched by MediarqJobDispatcher.");

        public Task Send(ICommand request, CancellationToken cancellationToken = default)
        {
            LastCommand = request;
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("Only void commands are dispatched by MediarqJobDispatcher.");
    }

    [Fact]
    public async Task DispatchAsync_Sends_The_Command_Through_The_Sender()
    {
        var sender = new RecordingSender();
        var dispatcher = new MediarqJobDispatcher(sender);
        var command = new TestCommand("x");

        await dispatcher.DispatchAsync(command);

        sender.LastCommand.Should().Be(command);
    }
}
