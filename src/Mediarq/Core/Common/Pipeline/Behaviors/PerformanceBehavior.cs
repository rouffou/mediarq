using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Time;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly IClock _clock;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, IClock clock)
    {
        _logger = logger;
        _clock = clock;
    }

    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var stopWatch = Stopwatch.StartNew();
        var response = await next();
        stopWatch.Stop();

        var elapsedMs = stopWatch.ElapsedMilliseconds;

        if(elapsedMs > 500)
        {
            _logger.LogWarning("Long running request: {RequestName} ({ElapsedMilliseconds} ms)", typeof(TRequest).Name, elapsedMs);
        }

        return response;
    }
}
