using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Common.Pipeline;

/// <summary>
/// Default implementation of <see cref="IPipelineExecutor"/> responsible for executing
/// all configured <see cref="IPipelineBehavior{TRequest, TResponse}"/> instances in sequence.
/// </summary>
/// <remarks>
/// The <see cref="PipelineExecutor"/> acts as the middleware coordinator within the Mediarq framework.
/// It builds a delegate chain that wraps the final request handler with any number of
/// registered pipeline behaviors (e.g., validation, logging, performance tracking, etc.).
/// 
/// Each behavior executes in reverse registration order—meaning the last registered behavior
/// will run closest to the handler—allowing developers to define pre- and post-processing logic
/// in a structured and composable way.
/// </remarks>
public class PipelineExecutor(IHandlerResolver handlerResolver) : IPipelineExecutor
{
    private readonly IHandlerResolver _handlerResolver = handlerResolver;

    /// <summary>
    /// Executes the request pipeline for a given request and response type.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The type of the request being processed. Must implement <see cref="ICommandOrQuery{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response returned after the request handler completes execution.
    /// </typeparam>
    /// <param name="context">
    /// The <see cref="RequestContext{TRequest, TResponse}"/> containing metadata, the request object,
    /// and other contextual information used throughout the pipeline.
    /// </param>
    /// <param name="handlerDelegate">
    /// The final delegate that represents the actual request handler logic to be invoked
    /// after all pipeline behaviors have executed.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that can be used to cancel the execution of the pipeline.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the entire pipeline
    /// and produces a response of type <typeparamref name="TResponse"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> or <paramref name="handlerDelegate"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// The pipeline execution process consists of the following steps:
    /// </para>
    /// <list type="number">
    ///   <item><description>Resolve all registered <see cref="IPipelineBehavior{TRequest, TResponse}"/> instances using the <see cref="IHandlerResolver"/>.</description></item>
    ///   <item><description>Wrap each behavior around the next delegate, forming a reverse-ordered chain of execution.</description></item>
    ///   <item><description>Invoke each behavior’s <see cref="IPipelineBehavior{TRequest, TResponse}.Handle"/> method in sequence.</description></item>
    ///   <item><description>Finally, call the provided <paramref name="handlerDelegate"/> to execute the request handler.</description></item>
    /// </list>
    ///
    /// Example:
    /// <code>
    /// var response = await pipelineExecutor.ExecuteAsync(
    ///     context,
    ///     ct => handler.Handle(request, ct),
    ///     cancellationToken);
    /// </code>
    ///
    /// This mechanism enables cross-cutting concerns such as logging, validation,
    /// performance monitoring, and authorization to be implemented in a modular fashion.
    /// </remarks>
    public Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        RequestContext<TRequest, TResponse> context,
        Func<CancellationToken, Task<TResponse>> handlerDelegate,
        CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(handlerDelegate);

        var behaviors = _handlerResolver.ResolveAll<IPipelineBehavior<TRequest, TResponse>>();
        var active = PipelineDispatch.SelectActive<TRequest, TResponse>(behaviors, out var activeCount);

        // No active behavior: invoke the handler directly, with none of the chain/closure overhead.
        if (active is null)
        {
            return handlerDelegate(cancellationToken);
        }

        return PipelineDispatch.Run(active, activeCount, context, handlerDelegate, cancellationToken);
    }

    /// <inheritdoc />
    public Task<TResponse> ExecuteAsync<TRequest, TResponse>(
        TRequest request,
        IRequestHandler<TRequest, TResponse> handler,
        IRequestContextFactory contextFactory,
        CancellationToken cancellationToken = default)
        where TRequest : ICommandOrQuery<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(contextFactory);

        var behaviors = _handlerResolver.ResolveAll<IPipelineBehavior<TRequest, TResponse>>();
        var active = PipelineDispatch.SelectActive<TRequest, TResponse>(behaviors, out var activeCount);

        // No active behavior: skip the request context allocation and the delegate entirely, invoking
        // the handler directly — the hot path costs no more than a bare handler call.
        if (active is null)
        {
            return handler.Handle(request, cancellationToken);
        }

        // A behavior will observe the context, so create it now (lazily, only when actually needed).
        RequestContext<TRequest, TResponse> context = contextFactory.Create<TRequest, TResponse>(request, cancellationToken);
        return PipelineDispatch.Run(active, activeCount, context, ct => handler.Handle(request, ct), cancellationToken);
    }
}
