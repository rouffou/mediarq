using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Mediators;

public class Mediator: IMediator
{
    private readonly ServiceFactory _serviceFactory;
    private readonly IRequestContextFactory _requestContextFactory;
    private readonly IPipelineExecutor _pipelineExecutor;

    public Mediator(
        ServiceFactory serviceFactory,
        IRequestContextFactory requestContextFactory,
        IPipelineExecutor pipelineExecutor)
    {
        ArgumentNullException.ThrowIfNull(serviceFactory);
        ArgumentNullException.ThrowIfNull(requestContextFactory);
        ArgumentNullException.ThrowIfNull(pipelineExecutor);

        _serviceFactory = serviceFactory;
        _requestContextFactory = requestContextFactory;
        _pipelineExecutor = pipelineExecutor;
    }

    public Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
                
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        dynamic handler = _serviceFactory(handlerType)
            ?? throw new InvalidOperationException($"No handler found for {request.GetType().Name}");
        
        Func<CancellationToken, Task<TResponse>> next = ct =>
        {
            dynamic h = handler;
            return h.Handle((dynamic)request, ct);
        };

        try
        {
            var requestContext = _requestContextFactory.Create<ICommandOrQuery<TResponse>, TResponse>(request, cancellationToken);

            var executeMethod = typeof(IPipelineExecutor)
                .GetMethod("ExecuteAsync")!
                .MakeGenericMethod(request.GetType(), typeof(TResponse));

            return (Task<TResponse>)executeMethod.Invoke(_pipelineExecutor, new object[] { requestContext, next, cancellationToken })!;
        }
        catch (Exception ex)
        {

            throw new InvalidOperationException(
                $"Error while handling request {request.GetType().Name}", ex);
        }
    }
}
