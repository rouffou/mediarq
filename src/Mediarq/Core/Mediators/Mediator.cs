using System.Collections.Concurrent;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Resolvers;

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
/// Per request type, a strongly-typed wrapper is built once and cached, so the dispatch path
/// does not rely on <c>dynamic</c> or per-call reflection.
///
/// This class is designed to replace MediatR with a lightweight, dependency-free mediator
/// that integrates easily into domain-driven and CQRS-based architectures.
/// </remarks>
public class Mediator : IMediator
{
    // Cached per concrete request type. A request type maps to exactly one response type, so the
    // request type alone is a sufficient cache key. Wrappers are stateless and thread-safe.
    private static readonly ConcurrentDictionary<Type, object> Wrappers = new();

    private readonly IRequestContextFactory _requestContextFactory;
    private readonly IPipelineExecutor _pipelineExecutor;
    private readonly IHandlerResolver _handlerResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="requestContextFactory">
    /// The factory used to create <see cref="RequestContext{TRequest, TResponse}"/> instances that encapsulate metadata about the request.
    /// </param>
    /// <param name="pipelineExecutor">
    /// The component responsible for executing the pipeline of behaviors and invoking the request handler.
    /// </param>
    /// <param name="handlerResolver">
    /// The resolver used to obtain handlers and behaviors from the dependency injection container.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the constructor parameters are <see langword="null"/>.
    /// </exception>
    public Mediator(
        IRequestContextFactory requestContextFactory,
        IPipelineExecutor pipelineExecutor,
        IHandlerResolver handlerResolver)
    {
        ArgumentNullException.ThrowIfNull(handlerResolver);
        ArgumentNullException.ThrowIfNull(requestContextFactory);
        ArgumentNullException.ThrowIfNull(pipelineExecutor);

        _requestContextFactory = requestContextFactory;
        _pipelineExecutor = pipelineExecutor;
        _handlerResolver = handlerResolver;
    }

    /// <inheritdoc />
    public Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapper = (IRequestHandlerWrapper<TResponse>)Wrappers.GetOrAdd(
            request.GetType(),
            requestType =>
            {
                var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
                return Activator.CreateInstance(wrapperType)
                    ?? throw new InvalidOperationException($"Could not create a handler wrapper for request type '{requestType}'.");
            });

        try
        {
            return wrapper.Handle(request, _handlerResolver, _requestContextFactory, _pipelineExecutor, cancellationToken);
        }
        catch (HandlerNotFoundException)
        {
            // A missing handler is a configuration error and is surfaced as-is, not wrapped.
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error while handling request {request.GetType().Name}", ex);
        }
    }

    /// <inheritdoc />
    public Task Send(ICommand request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // A no-result command is dispatched as a Unit request, so it flows through the same pipeline.
        return Send<Unit>(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handlers = _handlerResolver.ResolveAll(handlerType).ToList();

        if (handlers.Count == 0)
        {
            return;
        }

        var handleMethod = handlerType.GetMethod(nameof(INotificationHandler<TNotification>.Handle))!;
        var tasks = new List<Task>(handlers.Count);

        foreach (var handler in handlers)
        {
            tasks.Add((Task)handleMethod.Invoke(handler, [notification, cancellationToken])!);
        }

        await Task.WhenAll(tasks);
    }
}
