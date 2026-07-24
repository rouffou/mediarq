using FluentAssertions;
using Hangfire;
using Hangfire.InMemory;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mediarq.Hangfire.Tests;

/// <summary>
/// Exercises the full round trip through a real (in-memory) Hangfire storage and worker: the job created
/// by <see cref="HangfireSchedulingExtensions.Enqueue{TCommand}"/> targets a <em>generic</em> method
/// (<see cref="IMediarqJobDispatcher.DispatchAsync{TCommand}"/>) — this proves Hangfire can actually
/// serialize, persist, and later reconstruct that generic method call and its concrete-typed argument,
/// not just that the <see cref="Hangfire.Common.Job"/> shape looks right in isolation (the other tests in
/// this project use a fake <see cref="IBackgroundJobClient"/> and never touch real (de)serialization).
/// </summary>
public class EndToEndDispatchTests
{
    private sealed record GreetCommand(string Name) : ICommand;

    private sealed class RecordingSender : ISender
    {
        public static readonly List<object> ReceivedCommands = [];

        public Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task Send(ICommand request, CancellationToken cancellationToken = default)
        {
            ReceivedCommands.Add(request);
            return Task.CompletedTask;
        }

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    [Fact]
    public async Task Enqueue_Round_Trips_Through_Real_Hangfire_Storage_And_A_Real_Worker()
    {
        RecordingSender.ReceivedCommands.Clear();

        var services = new ServiceCollection();
        services.AddSingleton<ISender, RecordingSender>();
        services.AddMediarqHangfire();
        services.AddHangfire(cfg => cfg.UseInMemoryStorage());
        services.AddHangfireServer();
        await using var provider = services.BuildServiceProvider();

        var server = provider.GetRequiredService<IEnumerable<IHostedService>>()
            .OfType<BackgroundJobServerHostedService>()
            .Single();
        await server.StartAsync(CancellationToken.None);

        try
        {
            var client = provider.GetRequiredService<IBackgroundJobClient>();
            var command = new GreetCommand("World");

            client.Enqueue(command);

            var deadline = DateTime.UtcNow.AddSeconds(10);
            while (RecordingSender.ReceivedCommands.Count == 0 && DateTime.UtcNow < deadline)
            {
                await Task.Delay(50);
            }

            RecordingSender.ReceivedCommands.Should().ContainSingle().Which.Should().Be(command);
        }
        finally
        {
            await server.StopAsync(CancellationToken.None);
        }
    }
}
