using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.Logging;

namespace Mediarq.Core.Common.Pipeline.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        _logger.LogInformation("Handling {RequestType} with RequestId {RequestId} started at {StartedAt}", typeof(TRequest).Name, request.RequestId, request.StartedAt);

        var response = next();

        _logger.LogInformation("Handled {RequestType} with RequestId {RequestId} ended at {EndedAt}", typeof(TRequest).Name, request.RequestId, request.FinishedAt);

        return response;
    }
}
