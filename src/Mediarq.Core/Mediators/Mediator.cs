using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Mediarq.Core.Common.Contexts;
using Mediarq.Core.Common.Exceptions;
using Mediarq.Core.Common.Pipeline;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Streaming;
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
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> NotificationWrappers = new();
    private static readonly ConcurrentDictionary<Type, object> StreamWrappers = new();

    private readonly IRequestContextFactory _requestContextFactory;
    private readonly IHandlerResolver _handlerResolver;
    private readonly INotificationPublisher _notificationPublisher;
    private readonly MediarqWrapperRegistry? _wrapperRegistry;
    private readonly IStreamPipelineExecutor? _streamPipelineExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="requestContextFactory">
    /// The factory used to create <see cref="RequestContext{TRequest, TResponse}"/> instances that encapsulate metadata about the request.
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
    /// <param name="streamPipelineExecutor">
    /// Optional executor that wraps streaming requests with <see cref="IStreamPipelineBehavior{TRequest, TResponse}"/>.
    /// When <see langword="null"/>, <see cref="CreateStream{TResponse}"/> invokes the handler directly.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if any of the required constructor parameters are <see langword="null"/>.
    /// </exception>
    public Mediator(
        IRequestContextFactory requestContextFactory,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        MediarqWrapperRegistry? wrapperRegistry = null,
        IStreamPipelineExecutor? streamPipelineExecutor = null)
    {
        ArgumentNullException.ThrowIfNull(handlerResolver);
        ArgumentNullException.ThrowIfNull(requestContextFactory);
        ArgumentNullException.ThrowIfNull(notificationPublisher);

        _requestContextFactory = requestContextFactory;
        _handlerResolver = handlerResolver;
        _notificationPublisher = notificationPublisher;
        _wrapperRegistry = wrapperRegistry;
        _streamPipelineExecutor = streamPipelineExecutor;
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
            return wrapper.Handle(request, _handlerResolver, _requestContextFactory, cancellationToken);
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

    /// <inheritdoc />
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = FallbackJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = FallbackJustification)]
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var wrapper = _wrapperRegistry?.GetStreamWrapper<TResponse>(requestType)
            ?? GetOrCreateStreamWrapper<TResponse>(requestType);

        return wrapper.Handle(request, _handlerResolver, _streamPipelineExecutor, cancellationToken);
    }

    // Reflective fallback used only without a registry (assembly-scan mode). Built once per type and cached.
    [RequiresUnreferencedCode(FallbackJustification)]
    [RequiresDynamicCode(FallbackJustification)]
    private static RequestHandlerWrapper<TResponse> GetOrCreateRequestWrapper<TResponse>(Type requestType)
    {
        if (Wrappers.TryGetValue(requestType, out var existing))
        {
            return (RequestHandlerWrapper<TResponse>)existing;
        }

        var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
        var wrapper = Activator.CreateInstance(wrapperType)
            ?? throw new InvalidOperationException($"Could not create a handler wrapper for request type '{requestType}'.");

        return (RequestHandlerWrapper<TResponse>)Wrappers.GetOrAdd(requestType, wrapper);
    }

    // Reflective fallback used only without a registry (assembly-scan mode). Built once per type and cached.
    [RequiresUnreferencedCode(FallbackJustification)]
    [RequiresDynamicCode(FallbackJustification)]
    private static NotificationHandlerWrapper GetOrCreateNotificationWrapper(Type notificationType)
    {
        if (NotificationWrappers.TryGetValue(notificationType, out var existing))
        {
            return existing;
        }

        var wrapperType = typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(notificationType);
        var wrapper = (NotificationHandlerWrapper)(Activator.CreateInstance(wrapperType)
            ?? throw new InvalidOperationException($"Could not create a notification wrapper for type '{notificationType}'."));

        return NotificationWrappers.GetOrAdd(notificationType, wrapper);
    }

    // Reflective fallback used only without a registry (assembly-scan mode). Built once per type and cached.
    [RequiresUnreferencedCode(FallbackJustification)]
    [RequiresDynamicCode(FallbackJustification)]
    private static StreamRequestHandlerWrapper<TResponse> GetOrCreateStreamWrapper<TResponse>(Type requestType)
    {
        if (StreamWrappers.TryGetValue(requestType, out var existing))
        {
            return (StreamRequestHandlerWrapper<TResponse>)existing;
        }

        var wrapperType = typeof(StreamRequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
        var wrapper = Activator.CreateInstance(wrapperType)
            ?? throw new InvalidOperationException($"Could not create a stream handler wrapper for request type '{requestType}'.");

        return (StreamRequestHandlerWrapper<TResponse>)StreamWrappers.GetOrAdd(requestType, wrapper);
    }
}
