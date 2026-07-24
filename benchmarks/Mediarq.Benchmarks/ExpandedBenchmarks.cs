using BenchmarkDotNet.Attributes;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Registration;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Resolvers;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Common.Time;
using Mediarq.Core.Common.User;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.Extensions.DependencyInjection;
using Wolverine;
using MediarqMediator = Mediarq.Core.Mediators.IMediator;

// Extended benchmarks requested for a broader performance picture: a deep pipeline (many chained
// behaviors), scoped vs singleton handler lifetime, dispatch cost with many other handlers registered,
// and a cross-library comparison beyond MediatR (Wolverine, Brighter) on a plain void-command dispatch
// -- the one shape all four libraries support the same way, avoiding an apples-to-oranges comparison of
// each library's own request/response idiom.

/// <summary>
/// Dispatch through a deep pipeline (<see cref="Depth"/> chained passthrough behaviors), Mediarq vs
/// MediatR. Isolates per-behavior overhead from the base dispatch cost already measured by <see cref="SendBenchmarks"/>.
/// </summary>
[MemoryDiagnoser]
public class DeepPipelineBenchmarks
{
    private const int Depth = 10;

    private IServiceScope _mediarqScope = null!;
    private IServiceScope _mediatrScope = null!;
    private MediarqMediator _mediarq = null!;
    private MediatR.IMediator _mediatr = null!;

    [GlobalSetup]
    public void Setup()
    {
        // AddMediarqCore() (not the scanning AddMediarq) + explicit registration: the scanning path
        // would auto-discover MediarqPassthroughBehavior once via its assembly scan regardless of this
        // loop, making the exact chain depth harder to reason about.
        var mediarqServices = new ServiceCollection();
        mediarqServices.AddScoped<Mediarq.Core.Common.Resolvers.IHandlerResolver>(sp => new Mediarq.Core.Common.Resolvers.HandlerResolver(sp));
        mediarqServices.AddScoped<MediarqMediator, Mediator>();
        mediarqServices.AddSingleton<IClock, SystemClock>();
        mediarqServices.AddScoped<IRequestContextFactory, RequestContextFactory>();
        mediarqServices.AddScoped<IPipelineExecutor, PipelineExecutor>();
        mediarqServices.AddSingleton<Mediarq.Core.Common.Requests.Notifications.INotificationPublisher, Mediarq.Core.Common.Requests.Notifications.ParallelNotificationPublisher>();
        mediarqServices.AddScoped<IUserContext, DefaultUserContext>();
        mediarqServices.AddScoped<IRequestHandler<MediarqDeepPing, Result<string>>, MediarqDeepPingHandler>();
        for (var i = 0; i < Depth; i++)
        {
            mediarqServices.AddScoped(typeof(IPipelineBehavior<,>), typeof(MediarqPassthroughBehavior<,>));
        }

        _mediarqScope = mediarqServices.BuildServiceProvider().CreateScope();
        _mediarq = _mediarqScope.ServiceProvider.GetRequiredService<MediarqMediator>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddScoped<MediatR.IRequestHandler<MediatRDeepPing, string>, MediatRDeepPingHandler>();
        mediatrServices.AddScoped<MediatR.IMediator, MediatR.Mediator>();
        mediatrServices.AddScoped<MediatR.ISender>(sp => sp.GetRequiredService<MediatR.IMediator>());
        mediatrServices.AddScoped<MediatR.IPublisher>(sp => sp.GetRequiredService<MediatR.IMediator>());
        for (var i = 0; i < Depth; i++)
        {
            mediatrServices.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(MediatRPassthroughBehavior<,>));
        }

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
    public Task<string> MediatR_Send_DeepPipeline() => _mediatr.Send(new MediatRDeepPing("x"));

    [Benchmark]
    public async Task<string> Mediarq_Send_DeepPipeline()
    {
        Result<string> result = await _mediarq.Send(new MediarqDeepPing("x"));
        return result.Value;
    }
}

public sealed class MediarqPassthroughBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    public Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
        => handle();
}

public sealed class MediatRPassthroughBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, MediatR.RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next();
}

public record MediarqDeepPing(string Message) : ICommand<Result<string>>;

public sealed class MediarqDeepPingHandler : ICommandHandler<MediarqDeepPing, Result<string>>
{
    public Task<Result<string>> Handle(MediarqDeepPing request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Message));
}

public record MediatRDeepPing(string Message) : MediatR.IRequest<string>;

public sealed class MediatRDeepPingHandler : MediatR.IRequestHandler<MediatRDeepPing, string>
{
    public Task<string> Handle(MediatRDeepPing request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

/// <summary>
/// Mediarq only: a handler registered with the default Scoped lifetime vs one opted into Singleton via
/// <c>[RegisterHandler(ServiceLifetime.Singleton)]</c>, dispatched repeatedly within the same DI scope.
/// </summary>
[MemoryDiagnoser]
public class LifetimeBenchmarks
{
    private IServiceScope _scope = null!;
    private MediarqMediator _mediarq = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMediarq(isHttp: false, typeof(LifetimeBenchmarks).Assembly);
        _scope = services.BuildServiceProvider().CreateScope();
        _mediarq = _scope.ServiceProvider.GetRequiredService<MediarqMediator>();
    }

    [GlobalCleanup]
    public void Cleanup() => _scope.Dispose();

    [Benchmark(Baseline = true)]
    public async Task<string> Mediarq_Send_ScopedHandler()
    {
        Result<string> result = await _mediarq.Send(new ScopedPing("x"));
        return result.Value;
    }

    [Benchmark]
    public async Task<string> Mediarq_Send_SingletonHandler()
    {
        Result<string> result = await _mediarq.Send(new SingletonPing("x"));
        return result.Value;
    }
}

public record ScopedPing(string Message) : ICommand<Result<string>>;

public sealed class ScopedPingHandler : ICommandHandler<ScopedPing, Result<string>>
{
    public Task<Result<string>> Handle(ScopedPing request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Message));
}

public record SingletonPing(string Message) : ICommand<Result<string>>;

[RegisterHandler(ServiceLifetime.Singleton)]
public sealed class SingletonPingHandler : ICommandHandler<SingletonPing, Result<string>>
{
    public Task<Result<string>> Handle(SingletonPing request, CancellationToken cancellationToken = default)
        => Task.FromResult(Result.Success(request.Message));
}

/// <summary>
/// Dispatches to one specific handler (#15) out of 30 registered request/handler pairs, Mediarq vs
/// MediatR — isolates whether dispatch cost to a given request type grows with the total number of
/// other, unrelated handlers registered in the same container.
/// </summary>
[MemoryDiagnoser]
public class ManyHandlersBenchmarks
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
        mediarqServices.AddMediarq(isHttp: false, typeof(ManyHandlersBenchmarks).Assembly);
        _mediarqScope = mediarqServices.BuildServiceProvider().CreateScope();
        _mediarq = _mediarqScope.ServiceProvider.GetRequiredService<MediarqMediator>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ManyHandlersBenchmarks).Assembly));
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
    public Task<string> MediatR_Send_AmongManyHandlers() => _mediatr.Send(new MediatRManyHandlersPing15("x"));

    [Benchmark]
    public async Task<string> Mediarq_Send_AmongManyHandlers()
    {
        Result<string> result = await _mediarq.Send(new MediarqManyHandlersPing15("x"));
        return result.Value;
    }
}

public record MediarqManyHandlersPing1(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing1Handler : ICommandHandler<MediarqManyHandlersPing1, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing1 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing1(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing1Handler : MediatR.IRequestHandler<MediatRManyHandlersPing1, string>
{
    public Task<string> Handle(MediatRManyHandlersPing1 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing2(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing2Handler : ICommandHandler<MediarqManyHandlersPing2, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing2 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing2(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing2Handler : MediatR.IRequestHandler<MediatRManyHandlersPing2, string>
{
    public Task<string> Handle(MediatRManyHandlersPing2 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing3(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing3Handler : ICommandHandler<MediarqManyHandlersPing3, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing3 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing3(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing3Handler : MediatR.IRequestHandler<MediatRManyHandlersPing3, string>
{
    public Task<string> Handle(MediatRManyHandlersPing3 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing4(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing4Handler : ICommandHandler<MediarqManyHandlersPing4, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing4 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing4(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing4Handler : MediatR.IRequestHandler<MediatRManyHandlersPing4, string>
{
    public Task<string> Handle(MediatRManyHandlersPing4 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing5(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing5Handler : ICommandHandler<MediarqManyHandlersPing5, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing5 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing5(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing5Handler : MediatR.IRequestHandler<MediatRManyHandlersPing5, string>
{
    public Task<string> Handle(MediatRManyHandlersPing5 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing6(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing6Handler : ICommandHandler<MediarqManyHandlersPing6, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing6 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing6(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing6Handler : MediatR.IRequestHandler<MediatRManyHandlersPing6, string>
{
    public Task<string> Handle(MediatRManyHandlersPing6 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing7(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing7Handler : ICommandHandler<MediarqManyHandlersPing7, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing7 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing7(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing7Handler : MediatR.IRequestHandler<MediatRManyHandlersPing7, string>
{
    public Task<string> Handle(MediatRManyHandlersPing7 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing8(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing8Handler : ICommandHandler<MediarqManyHandlersPing8, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing8 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing8(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing8Handler : MediatR.IRequestHandler<MediatRManyHandlersPing8, string>
{
    public Task<string> Handle(MediatRManyHandlersPing8 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing9(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing9Handler : ICommandHandler<MediarqManyHandlersPing9, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing9 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing9(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing9Handler : MediatR.IRequestHandler<MediatRManyHandlersPing9, string>
{
    public Task<string> Handle(MediatRManyHandlersPing9 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing10(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing10Handler : ICommandHandler<MediarqManyHandlersPing10, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing10 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing10(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing10Handler : MediatR.IRequestHandler<MediatRManyHandlersPing10, string>
{
    public Task<string> Handle(MediatRManyHandlersPing10 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing11(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing11Handler : ICommandHandler<MediarqManyHandlersPing11, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing11 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing11(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing11Handler : MediatR.IRequestHandler<MediatRManyHandlersPing11, string>
{
    public Task<string> Handle(MediatRManyHandlersPing11 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing12(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing12Handler : ICommandHandler<MediarqManyHandlersPing12, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing12 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing12(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing12Handler : MediatR.IRequestHandler<MediatRManyHandlersPing12, string>
{
    public Task<string> Handle(MediatRManyHandlersPing12 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing13(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing13Handler : ICommandHandler<MediarqManyHandlersPing13, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing13 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing13(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing13Handler : MediatR.IRequestHandler<MediatRManyHandlersPing13, string>
{
    public Task<string> Handle(MediatRManyHandlersPing13 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing14(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing14Handler : ICommandHandler<MediarqManyHandlersPing14, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing14 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing14(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing14Handler : MediatR.IRequestHandler<MediatRManyHandlersPing14, string>
{
    public Task<string> Handle(MediatRManyHandlersPing14 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing15(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing15Handler : ICommandHandler<MediarqManyHandlersPing15, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing15 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing15(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing15Handler : MediatR.IRequestHandler<MediatRManyHandlersPing15, string>
{
    public Task<string> Handle(MediatRManyHandlersPing15 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing16(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing16Handler : ICommandHandler<MediarqManyHandlersPing16, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing16 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing16(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing16Handler : MediatR.IRequestHandler<MediatRManyHandlersPing16, string>
{
    public Task<string> Handle(MediatRManyHandlersPing16 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing17(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing17Handler : ICommandHandler<MediarqManyHandlersPing17, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing17 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing17(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing17Handler : MediatR.IRequestHandler<MediatRManyHandlersPing17, string>
{
    public Task<string> Handle(MediatRManyHandlersPing17 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing18(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing18Handler : ICommandHandler<MediarqManyHandlersPing18, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing18 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing18(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing18Handler : MediatR.IRequestHandler<MediatRManyHandlersPing18, string>
{
    public Task<string> Handle(MediatRManyHandlersPing18 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing19(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing19Handler : ICommandHandler<MediarqManyHandlersPing19, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing19 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing19(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing19Handler : MediatR.IRequestHandler<MediatRManyHandlersPing19, string>
{
    public Task<string> Handle(MediatRManyHandlersPing19 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing20(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing20Handler : ICommandHandler<MediarqManyHandlersPing20, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing20 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing20(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing20Handler : MediatR.IRequestHandler<MediatRManyHandlersPing20, string>
{
    public Task<string> Handle(MediatRManyHandlersPing20 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing21(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing21Handler : ICommandHandler<MediarqManyHandlersPing21, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing21 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing21(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing21Handler : MediatR.IRequestHandler<MediatRManyHandlersPing21, string>
{
    public Task<string> Handle(MediatRManyHandlersPing21 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing22(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing22Handler : ICommandHandler<MediarqManyHandlersPing22, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing22 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing22(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing22Handler : MediatR.IRequestHandler<MediatRManyHandlersPing22, string>
{
    public Task<string> Handle(MediatRManyHandlersPing22 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing23(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing23Handler : ICommandHandler<MediarqManyHandlersPing23, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing23 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing23(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing23Handler : MediatR.IRequestHandler<MediatRManyHandlersPing23, string>
{
    public Task<string> Handle(MediatRManyHandlersPing23 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing24(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing24Handler : ICommandHandler<MediarqManyHandlersPing24, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing24 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing24(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing24Handler : MediatR.IRequestHandler<MediatRManyHandlersPing24, string>
{
    public Task<string> Handle(MediatRManyHandlersPing24 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing25(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing25Handler : ICommandHandler<MediarqManyHandlersPing25, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing25 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing25(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing25Handler : MediatR.IRequestHandler<MediatRManyHandlersPing25, string>
{
    public Task<string> Handle(MediatRManyHandlersPing25 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing26(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing26Handler : ICommandHandler<MediarqManyHandlersPing26, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing26 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing26(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing26Handler : MediatR.IRequestHandler<MediatRManyHandlersPing26, string>
{
    public Task<string> Handle(MediatRManyHandlersPing26 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing27(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing27Handler : ICommandHandler<MediarqManyHandlersPing27, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing27 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing27(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing27Handler : MediatR.IRequestHandler<MediatRManyHandlersPing27, string>
{
    public Task<string> Handle(MediatRManyHandlersPing27 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing28(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing28Handler : ICommandHandler<MediarqManyHandlersPing28, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing28 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing28(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing28Handler : MediatR.IRequestHandler<MediatRManyHandlersPing28, string>
{
    public Task<string> Handle(MediatRManyHandlersPing28 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing29(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing29Handler : ICommandHandler<MediarqManyHandlersPing29, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing29 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing29(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing29Handler : MediatR.IRequestHandler<MediatRManyHandlersPing29, string>
{
    public Task<string> Handle(MediatRManyHandlersPing29 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

public record MediarqManyHandlersPing30(string Message) : ICommand<Result<string>>;
public sealed class MediarqManyHandlersPing30Handler : ICommandHandler<MediarqManyHandlersPing30, Result<string>>
{
    public Task<Result<string>> Handle(MediarqManyHandlersPing30 request, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success(request.Message));
}

public record MediatRManyHandlersPing30(string Message) : MediatR.IRequest<string>;
public sealed class MediatRManyHandlersPing30Handler : MediatR.IRequestHandler<MediatRManyHandlersPing30, string>
{
    public Task<string> Handle(MediatRManyHandlersPing30 request, CancellationToken cancellationToken) => Task.FromResult(request.Message);
}

/// <summary>
/// Cross-library comparison beyond MediatR: Mediarq vs MediatR vs Wolverine vs Brighter, dispatching a
/// plain void command/message with no return value — the one shape all four support the same way.
/// Each library has its own richer request/response idiom (Wolverine's <c>InvokeAsync&lt;T&gt;</c>,
/// Brighter's RPC-style <c>Call</c>), but comparing those directly would compare different concepts,
/// not the same operation across libraries.
/// </summary>
[MemoryDiagnoser]
public class CrossLibraryBenchmarks
{
    private IServiceScope _mediarqScope = null!;
    private IServiceScope _mediatrScope = null!;
    private Microsoft.Extensions.Hosting.IHost _wolverineHost = null!;
    private ServiceProvider _brighterProvider = null!;
    private MediarqMediator _mediarq = null!;
    private MediatR.IMediator _mediatr = null!;
    private Wolverine.IMessageBus _wolverine = null!;
    private Paramore.Brighter.IAmACommandProcessor _brighter = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var mediarqServices = new ServiceCollection();
        mediarqServices.AddLogging();
        mediarqServices.AddMediarq(isHttp: false, typeof(CrossLibraryBenchmarks).Assembly);
        _mediarqScope = mediarqServices.BuildServiceProvider().CreateScope();
        _mediarq = _mediarqScope.ServiceProvider.GetRequiredService<MediarqMediator>();

        var mediatrServices = new ServiceCollection();
        mediatrServices.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CrossLibraryBenchmarks).Assembly));
        _mediatrScope = mediatrServices.BuildServiceProvider().CreateScope();
        _mediatr = _mediatrScope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

        // Wolverine needs the full generic-host lifecycle -- even purely local, in-process InvokeAsync
        // throws WolverineHasNotStartedException unless the host was actually started.
        var wolverineBuilder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();
        wolverineBuilder.Services.AddWolverine(opts =>
        {
            opts.Discovery.IncludeAssembly(typeof(CrossLibraryBenchmarks).Assembly);
        });
        _wolverineHost = wolverineBuilder.Build();
        await _wolverineHost.StartAsync();
        _wolverine = _wolverineHost.Services.GetRequiredService<Wolverine.IMessageBus>();

        var brighterServices = new ServiceCollection();
        brighterServices.AddLogging();
        brighterServices.AddBrighter(_ => { }).AutoFromAssemblies([typeof(CrossLibraryBenchmarks).Assembly]);
        _brighterProvider = brighterServices.BuildServiceProvider();
        _brighter = _brighterProvider.GetRequiredService<Paramore.Brighter.IAmACommandProcessor>();
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        _mediarqScope.Dispose();
        _mediatrScope.Dispose();
        await _wolverineHost.StopAsync();
        _wolverineHost.Dispose();
        await _brighterProvider.DisposeAsync();
    }

    [Benchmark(Baseline = true)]
    public Task MediatR_Send_Void() => _mediatr.Send(new MediatRVoidPing());

    [Benchmark]
    public Task Mediarq_Send_Void() => _mediarq.Send(new MediarqVoidPing());

    [Benchmark]
    public Task Wolverine_InvokeAsync_Void() => _wolverine.InvokeAsync(new WolverineVoidPing());

    [Benchmark]
    public void Brighter_Send_Void() => _brighter.Send(new BrighterVoidPing());
}

public record MediarqVoidPing : ICommand;

public sealed class MediarqVoidPingHandler : ICommandHandler<MediarqVoidPing>
{
    public Task Handle(MediarqVoidPing request, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public record MediatRVoidPing : MediatR.IRequest;

public sealed class MediatRVoidPingHandler : MediatR.IRequestHandler<MediatRVoidPing>
{
    public Task Handle(MediatRVoidPing request, CancellationToken cancellationToken) => Task.CompletedTask;
}

public record WolverineVoidPing;

public static class WolverineVoidPingHandler
{
    public static void Handle(WolverineVoidPing message)
    {
    }
}

public class BrighterVoidPing() : Paramore.Brighter.Command(Paramore.Brighter.Id.Random());

public class BrighterVoidPingHandler : Paramore.Brighter.RequestHandler<BrighterVoidPing>
{
    public override BrighterVoidPing Handle(BrighterVoidPing command) => base.Handle(command);
}
