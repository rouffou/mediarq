using FluentAssertions;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Streaming;
using Mediarq.Core.Mediators;
using Microsoft.Extensions.DependencyInjection;

namespace Mediarq.Hangfire.Tests;

public class HangfireServiceCollectionExtensionsTests
{
    private sealed class CustomDispatcher : IMediarqJobDispatcher
    {
        public Task DispatchAsync<TCommand>(TCommand command)
            where TCommand : ICommand
            => Task.CompletedTask;
    }

    private sealed class StubSender : ISender
    {
        public Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task Send(ICommand request, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    [Fact]
    public void AddMediarqHangfire_Registers_The_Default_Dispatcher()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISender, StubSender>();

        services.AddMediarqHangfire();
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IMediarqJobDispatcher>().Should().BeOfType<MediarqJobDispatcher>();
    }

    [Fact]
    public void AddMediarqHangfire_Does_Not_Override_An_Already_Registered_Dispatcher()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IMediarqJobDispatcher, CustomDispatcher>();

        services.AddMediarqHangfire();
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IMediarqJobDispatcher>().Should().BeOfType<CustomDispatcher>();
    }

    [Fact]
    public void AddMediarqHangfire_Throws_For_A_Null_Services_Collection()
    {
        IServiceCollection services = null!;

        var act = () => services.AddMediarqHangfire();

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediarqHangfire_Returns_The_Same_Service_Collection()
    {
        var services = new ServiceCollection();

        var result = services.AddMediarqHangfire();

        result.Should().BeSameAs(services);
    }
}
