using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Provides a concrete implementation of the <see cref="IMediator"/> interface,
/// responsible for dispatching commands and queries to their corresponding handlers,
/// while executing registered pipeline behaviors (e.g. validation, logging, performance tracking).
/// </summary>
/// <remarks>
/// The <see cref="Mediator"/> acts as the central coordination point for request handling.
/// It abstracts away the complexity of handler resolution, context creation, and pipeline execution.
/// 
/// This class is designed to replace MediatR with a lightweight, dependency-free mediator
/// that integrates easily into domain-driven and CQRS-based architectures.
/// </remarks>
/// <example>
/// Example usage:
/// <code>
/// var serviceFactory = type => container.Resolve(type);
/// var requestContextFactory = new DefaultRequestContextFactory(userContext, clock);
/// var pipelineExecutor = new PipelineExecutor(serviceFactory);
///
/// var mediator = new Mediator(serviceFactory, requestContextFactory, pipelineExecutor);
///
/// var result = await mediator.Send(new CreateUserCommand("Alice"));
/// </code>
/// </example>
public class Mediator: IMediator
{
    private readonly ServiceFactory _serviceFactory;
    private readonly IRequestContextFactory _requestContextFactory;
    private readonly IPipelineExecutor _pipelineExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceFactory">
    /// A factory delegate responsible for resolving service instances, such as handlers or pipeline behaviors.
    /// </param>
    /// <param name="requestContextFactory">
    /// The factory used to create <see cref="RequestContext{TRequest, TResponse}"/> instances that encapsulate metadata about the request.
    /// </param>
    /// <param name="pipelineExecutor">
    /// The component responsible for executing the pipeline of behaviors and invoking the request handler.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the constructor parameters are <see langword="null"/>.
    /// </exception>
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

    /// <summary>
    /// Sends a command or query through the mediator pipeline and invokes the corresponding handler.
    /// </summary>
    /// <typeparam name="TResponse">
    /// The type of response expected from the handler. 
    /// Usually a <see cref="Result"/> or <see cref="Result{T}"/> instance.
    /// </typeparam>
    /// <param name="request">
    /// The command or query to process, implementing <see cref="ICommandOrQuery{TResponse}"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token for cooperative task cancellation.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, containing the response of type <typeparamref name="TResponse"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the <paramref name="request"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no matching handler can be found for the given request type,
    /// or if an exception occurs during handler invocation or pipeline execution.
    /// </exception>
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
