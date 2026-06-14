using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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
/// Dispatch goes through strongly-typed wrappers, so the path does not rely on <c>dynamic</c> or
/// per-call reflection. When a <see cref="MediarqWrapperRegistry"/> is supplied (populated by the
/// compile-time generated <c>AddMediarqHandlers()</c>), the wrappers are resolved with no reflection
/// at all — trimming/AOT friendly. Otherwise (the reflection-based <c>AddMediarq</c> assembly scan),
/// each wrapper is built reflectively once per type and cached.
///
/// This class is designed to replace MediatR with a lightweight, dependency-free mediator
/// that integrates easily into domain-driven and CQRS-based architectures.
/// </remarks>
public class Mediator : IMediator
{
    private const string FallbackJustification =
        "The reflective wrapper fallback is only reached when no source-generated MediarqWrapperRegistry " +
        "is present (the reflection-based AddMediarq assembly scan), which is not used on Native AOT. " +
        "The AOT path uses AddMediarqCore() + the generated AddMediarqHandlers(), which pre-populates the registry.";

    // Fallback caches, keyed per concrete request/notification type. Only used when no registry is
    // supplied. Wrappers are stateless and thread-safe.
    private static readonly ConcurrentDictionary<Type, object> Wrappers = new();
    private static readonly ConcurrentDictionary<Type, INotificationHandlerWrapper> NotificationWrappers = new();

    private readonly IRequestContextFactory _requestContextFactory;
    private readonly IPipelineExecutor _pipelineExecutor;
    private readonly IHandlerResolver _handlerResolver;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly MediarqWrapperRegistry? _wrapperRegistry;

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
    /// <param name="notificationPublisher">
    /// The strategy used to invoke notification handlers when publishing.
    /// </param>
    /// <param name="wrapperRegistry">
    /// Optional registry of compile-time generated dispatch wrappers. When supplied, request and
    /// notification dispatch is fully reflection-free. When <see langword="null"/>, wrappers are
    /// built reflectively on first use and cached.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the required constructor parameters are <see langword="null"/>.
    /// </exception>
    public Mediator(
        IRequestContextFactory requestContextFactory,
        IPipelineExecutor pipelineExecutor,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        MediarqWrapperRegistry? wrapperRegistry = null)
    {
        ArgumentNullException.ThrowIfNull(handlerResolver);
        ArgumentNullException.ThrowIfNull(requestContextFactory);
        ArgumentNullException.ThrowIfNull(pipelineExecutor);
        ArgumentNullException.ThrowIfNull(notificationPublisher);

        _requestContextFactory = requestContextFactory;
        _pipelineExecutor = pipelineExecutor;
        _handlerResolver = handlerResolver;
        _notificationPublisher = notificationPublisher;
        _wrapperRegistry = wrapperRegistry;
    }

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = FallbackJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = FallbackJustification)]
    public Task<TResponse> Send<TResponse>(ICommandOrQuery<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var wrapper = _wrapperRegistry?.GetRequestWrapper<TResponse>(requestType)
            ?? GetOrCreateRequestWrapper<TResponse>(requestType);

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
            throw new InvalidOperationException($"Error while handling request {requestType.Name}", ex);
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
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = FallbackJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = FallbackJustification)]
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var notificationType = notification.GetType();
        var wrapper = _wrapperRegistry?.GetNotificationWrapper(notificationType)
            ?? GetOrCreateNotificationWrapper(notificationType);

        return wrapper.Handle(notification, _handlerResolver, _notificationPublisher, cancellationToken);
    }

    // Reflective fallback used only without a registry (assembly-scan mode). Built once per type and cached.
    [RequiresUnreferencedCode(FallbackJustification)]
    [RequiresDynamicCode(FallbackJustification)]
    private static IRequestHandlerWrapper<TResponse> GetOrCreateRequestWrapper<TResponse>(Type requestType)
    {
        if (Wrappers.TryGetValue(requestType, out var existing))
        {
            return (IRequestHandlerWrapper<TResponse>)existing;
        }

        var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
        var wrapper = Activator.CreateInstance(wrapperType)
            ?? throw new InvalidOperationException($"Could not create a handler wrapper for request type '{requestType}'.");

        return (IRequestHandlerWrapper<TResponse>)Wrappers.GetOrAdd(requestType, wrapper);
    }

    // Reflective fallback used only without a registry (assembly-scan mode). Built once per type and cached.
    [RequiresUnreferencedCode(FallbackJustification)]
    [RequiresDynamicCode(FallbackJustification)]
    private static INotificationHandlerWrapper GetOrCreateNotificationWrapper(Type notificationType)
    {
        if (NotificationWrappers.TryGetValue(notificationType, out var existing))
        {
            return existing;
        }

        var wrapperType = typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(notificationType);
        var wrapper = (INotificationHandlerWrapper)(Activator.CreateInstance(wrapperType)
            ?? throw new InvalidOperationException($"Could not create a notification wrapper for type '{notificationType}'."));

        return NotificationWrappers.GetOrAdd(notificationType, wrapper);
    }
}
