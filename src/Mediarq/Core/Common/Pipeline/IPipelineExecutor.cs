using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline;

public interface IPipelineExecutor
{
    Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        RequestContext<TRequest, TResponse> context,
        Func<CancellationToken, Task<TResponse>> handlerDelegate,
        CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>;
}
