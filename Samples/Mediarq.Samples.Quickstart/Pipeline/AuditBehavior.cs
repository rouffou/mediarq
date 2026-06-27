using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Microsoft.Extensions.Logging;

namespace Mediarq.Samples.Quickstart.Pipeline;

/// <summary>
/// A custom cross-cutting behavior that wraps every command/query. Implements <see cref="IOrderBehavior"/>
/// to control where it sits in the pipeline — a lower <see cref="Order"/> runs first (outermost).
/// </summary>
public sealed class AuditBehavior<TRequest, TResponse>(ILogger<AuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>, IOrderBehavior
    where TRequest : ICommandOrQuery<TResponse>
{
    public int Order => 100;

    public async Task<TResponse> Handle(
        IMutableRequestContext<TRequest, TResponse> context,
        Func<Task<TResponse>> handle,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[audit] -> {Request}", typeof(TRequest).Name);
        var response = await handle();
        logger.LogInformation("[audit] <- {Request}", typeof(TRequest).Name);
        return response;
    }
}
