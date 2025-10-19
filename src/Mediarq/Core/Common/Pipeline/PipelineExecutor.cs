using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Common.Pipeline;

public class PipelineExecutor : IPipelineExecutor
{
    private readonly ServiceFactory _serviceFactory;

    public PipelineExecutor(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory;
    }

    public Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        RequestContext<TRequest, TResponse> context,
        Func<CancellationToken, Task<TResponse>> handlerDelegate,
        CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handlerDelegate);

        var behaviors = _serviceFactory(typeof(IEnumerable<IPipelineBehavior<TRequest, TResponse>>))
            as IEnumerable<IPipelineBehavior<TRequest, TResponse>>
            ?? Enumerable.Empty<IPipelineBehavior<TRequest, TResponse>>();

        Func<Task<TResponse>> next = () => handlerDelegate(cancellationToken);

        foreach (var behavior in behaviors.Reverse())
        {
            var currentNext = next;
            next = () => behavior.Handle(context, currentNext, cancellationToken);
        }

        return next();
    }
}
