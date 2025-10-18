using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline;

public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommandOrQuery<TResponse>
{
    Task<TResponse> Handle(IMutableRequestContext<TRequest, TResponse> request, Func<Task<TResponse>> next, CancellationToken cancellationToken = default);        
}
