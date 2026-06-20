using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Samples.Behaviors;

/// <summary>
/// Example custom pipeline behavior: logs each request around the handler. Implements
/// <see cref="IOrderBehavior"/> to control where it sits in the pipeline (lower runs first/outermost).
/// </summary>
public sealed class RequestAuditBehavior<TRequest, TResponse>(ILogger<RequestAuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    public int Order => 100;

    public async Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> context, Func<Task<TResponse>> handle, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[audit] handling {Request}", typeof(TRequest).Name);
        var response = await handle();
        logger.LogInformation("[audit] handled {Request}", typeof(TRequest).Name);
        return response;
    }
}
