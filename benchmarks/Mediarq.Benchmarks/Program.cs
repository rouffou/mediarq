using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Common.Time;
using Mediarq.Core.Common.User;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MediarqMediator = Mediarq.Core.Mediators.IMediator;

// Benchmarks must run in Release for reliable numbers: `dotnet run -c Release --project benchmarks/Mediarq.Benchmarks`.
// In a DEBUG build we relax the optimizations validator so the benchmarks can still be launched for
// debugging (results are NOT reliable in that case).
var config = DefaultConfig.Instance;
#if DEBUG
config = config.WithOptions(ConfigOptions.DisableOptimizationsValidator);
#endif

BenchmarkRunner.Run([typeof(SendBenchmarks), typeof(PublishBenchmarks)], config);

/// <summary>
/// Compares dispatching a request through Mediarq vs MediatR. Run in Release:
/// <c>dotnet run -c Release --project benchmarks/Mediarq.Benchmarks</c>.
/// </summary>
[MemoryDiagnoser]
public class SendBenchmarks
{
    private IServiceScope _mediarqScope = null!;
    private IServiceScope _mediarqLeanScope = null!;
    private IServiceScope _mediatrScope = null!;
    private MediarqMediator _mediarq = null!;
    private MediarqMediator _mediarqLean = null!;
    private MediatR.IMediator _mediatr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var mediarqServices = new ServiceCollection();
        mediarqServices.AddLogging();
        mediarqServices.AddMediarq(isHttp: false, typeof(SendBenchmarks).Assembly);
        _mediarqScope = mediarqServices.BuildServiceProvider().CreateScope();
        _mediarq = _mediarqScope.ServiceProvider.GetRequiredService<MediarqMediator>();

        // Lean Mediarq: core services + handler only, no pipeline behaviors registered. Isolates the
        // framework dispatch floor from the per-call cost of resolving the built-in behaviors.
        var leanServices = new ServiceCollection();
        leanServices.AddScoped<IHandlerResolver>(sp => new HandlerResolver(sp.GetService));
        leanServices.AddScoped<MediarqMediator, Mediator>();
        leanServices.AddSingleton<IClock, SystemClock>();
        leanServices.AddScoped<IRequestContextFactory, RequestContextFactory>();
        leanServices.AddScoped<IPipelineExecutor, PipelineExecutor>();
        leanServices.AddSingleton<INotificationPublisher, ParallelNotificationPublisher>();
        leanServices.AddScoped<IUserContext, DefaultUserContext>();
        leanServices.AddScoped<IRequestHandler<MediarqPing, Result<string>>, MediarqPingHandler>();
        _mediarqLeanScope = leanServices.BuildServiceProvider().CreateScope();
        _mediarqLean = _mediarqLeanScope.ServiceProvider.GetRequiredService<MediarqMediator>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendBenchmarks).Assembly));
        _mediatrScope = mediatrServices.BuildServiceProvider().CreateScope();
        _mediatr = _mediatrScope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _mediarqScope.Dispose();
        _mediarqLeanScope.Dispose();
        _mediatrScope.Dispose();
    }

    [Benchmark(Baseline = true)]
    public Task<string> MediatR_Send() => _mediatr.Send(new MediatRPing("x"));

    [Benchmark]
    public async Task<string> Mediarq_Send()
    {
        Result<string> result = await _mediarq.Send(new MediarqPing("x"));
        return result.Value;
    }

    [Benchmark]
    public async Task<string> Mediarq_Send_Lean()
    {
        Result<string> result = await _mediarqLean.Send(new MediarqPing("x"));
        return result.Value;
    }
}

public record MediarqPing(string Message) : ICommand<Result<string>>;

public sealed class MediarqPingHandler : ICommandHandler<MediarqPing, Result<string>>
{
    public Task<Result<string>> Handle(MediarqPing request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Message));
}

public record MediatRPing(string Message) : MediatR.IRequest<string>;

public sealed class MediatRPingHandler : MediatR.IRequestHandler<MediatRPing, string>
{
    public Task<string> Handle(MediatRPing request, CancellationToken cancellationToken)
        => Task.FromResult(request.Message);
}

/// <summary>
/// Compares publishing a notification to two handlers through Mediarq vs MediatR.
/// </summary>
[MemoryDiagnoser]
public class PublishBenchmarks
{
    private IServiceScope _mediarqScope = null!;
    private IServiceScope _mediatrScope = null!;
    private MediarqMediator _mediarq = null!;
    private MediatR.IMediator _mediatr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var mediarqServices = new ServiceCollection();
        mediarqServices.AddLogging();
        mediarqServices.AddMediarq(isHttp: false, typeof(PublishBenchmarks).Assembly);
        _mediarqScope = mediarqServices.BuildServiceProvider().CreateScope();
        _mediarq = _mediarqScope.ServiceProvider.GetRequiredService<MediarqMediator>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PublishBenchmarks).Assembly));
        _mediatrScope = mediatrServices.BuildServiceProvider().CreateScope();
        _mediatr = _mediatrScope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _mediarqScope.Dispose();
        _mediatrScope.Dispose();
    }

    [Benchmark(Baseline = true)]
    public Task MediatR_Publish() => _mediatr.Publish(new MediatRPinged(1));

    [Benchmark]
    public Task Mediarq_Publish() => _mediarq.Publish(new MediarqPinged(1));
}

public record MediarqPinged(int Id) : INotification;

public sealed class MediarqPingedHandlerA : INotificationHandler<MediarqPinged>
{
    public Task Handle(MediarqPinged notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public sealed class MediarqPingedHandlerB : INotificationHandler<MediarqPinged>
{
    public Task Handle(MediarqPinged notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public record MediatRPinged(int Id) : MediatR.INotification;

public sealed class MediatRPingedHandlerA : MediatR.INotificationHandler<MediatRPinged>
{
    public Task Handle(MediatRPinged notification, CancellationToken cancellationToken) => Task.CompletedTask;
}

public sealed class MediatRPingedHandlerB : MediatR.INotificationHandler<MediatRPinged>
{
    public Task Handle(MediatRPinged notification, CancellationToken cancellationToken) => Task.CompletedTask;
}
