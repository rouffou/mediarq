using FluentAssertions;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Mediarq.Quartz.Tests;

/// <summary>
/// Exercises the full round trip through a real Quartz scheduler and a real worker: the command is
/// JSON-serialized into <see cref="Quartz.JobDataMap"/> by <see cref="QuartzSchedulingExtensions"/> and
/// reconstructed by <see cref="MediarqQuartzJob"/> when the trigger fires — this proves the
/// type-name-based round trip actually works, not just that the job data looks right in isolation.
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
    public async Task Enqueue_Round_Trips_Through_A_Real_Scheduler_And_A_Real_Worker()
    {
        RecordingSender.ReceivedCommands.Clear();

        // QuartzHostedService needs the full generic-host lifecycle (IHostApplicationLifetime) -- a raw
        // ServiceCollection isn't enough, it throws resolving the hosted service.
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton<ISender, RecordingSender>();
        builder.Services.AddMediarqQuartz();
        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService();
        using var host = builder.Build();

        await host.StartAsync();

        try
        {
            var scheduler = await host.Services.GetRequiredService<ISchedulerFactory>().GetScheduler();
            var command = new GreetCommand("World");

            await scheduler.EnqueueAsync(command);

            var deadline = DateTime.UtcNow.AddSeconds(10);
            while (RecordingSender.ReceivedCommands.Count == 0 && DateTime.UtcNow < deadline)
            {
                await Task.Delay(50);
            }

            RecordingSender.ReceivedCommands.Should().ContainSingle().Which.Should().Be(command);
        }
        finally
        {
            await host.StopAsync();
        }
    }
}
