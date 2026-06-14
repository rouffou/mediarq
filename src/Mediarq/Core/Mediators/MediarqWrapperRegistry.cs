using System.Collections.Concurrent;
using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Holds the strongly-typed dispatch wrappers used by the <see cref="Mediator"/>, instantiated
/// without reflection. The compile-time generated <c>AddMediarqHandlers()</c> extension populates
/// this registry by calling <see cref="Add{TRequest, TResponse}"/> and
/// <see cref="AddNotification{TNotification}"/> for every discovered handler, so the dispatch path
/// never needs <c>Activator.CreateInstance</c> or <c>MakeGenericType</c> (trimming/AOT friendly).
/// </summary>
/// <remarks>
/// When the registry is absent (for example with the reflection-based <c>AddMediarq</c> assembly
/// scan), the mediator falls back to building the wrappers reflectively and caching them.
/// </remarks>
public sealed class MediarqWrapperRegistry
{
    private readonly ConcurrentDictionary<Type, object> _requestWrappers = new();
    private readonly ConcurrentDictionary<Type, INotificationHandlerWrapper> _notificationWrappers = new();

    /// <summary>
    /// Registers the dispatch wrapper for a request type. Called by generated code with both type
    /// arguments known, so the wrapper is instantiated with no reflection.
    /// </summary>
    /// <typeparam name="TRequest">The concrete request type.</typeparam>
    /// <typeparam name="TResponse">The response type produced by the request.</typeparam>
    /// <returns>The same registry, enabling fluent chaining.</returns>
    public MediarqWrapperRegistry Add<TRequest, TResponse>()
        where TRequest : ICommandOrQuery<TResponse>
    {
        _requestWrappers.TryAdd(typeof(TRequest), new RequestHandlerWrapperImpl<TRequest, TResponse>());
        return this;
    }

    /// <summary>
    /// Registers the dispatch wrapper for a notification type. Called by generated code with the
    /// notification type known, so the wrapper is instantiated with no reflection.
    /// </summary>
    /// <typeparam name="TNotification">The concrete notification type.</typeparam>
    /// <returns>The same registry, enabling fluent chaining.</returns>
    public MediarqWrapperRegistry AddNotification<TNotification>()
        where TNotification : INotification
    {
        _notificationWrappers.TryAdd(typeof(TNotification), new NotificationHandlerWrapperImpl<TNotification>());
        return this;
    }

    /// <summary>Gets the cached request wrapper for <paramref name="requestType"/>, if registered.</summary>
    internal IRequestHandlerWrapper<TResponse>? GetRequestWrapper<TResponse>(Type requestType)
        => _requestWrappers.TryGetValue(requestType, out var wrapper)
            ? (IRequestHandlerWrapper<TResponse>)wrapper
            : null;

    /// <summary>Gets the cached notification wrapper for <paramref name="notificationType"/>, if registered.</summary>
    internal INotificationHandlerWrapper? GetNotificationWrapper(Type notificationType)
        => _notificationWrappers.TryGetValue(notificationType, out var wrapper) ? wrapper : null;
}
