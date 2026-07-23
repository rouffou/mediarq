using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Streaming;

namespace Mediarq.MediatRCompat;

// One adapter per compat wrapper: resolved from DI with the user's original MediatR-shaped handler
// injected, and forwards the call, unwrapping the wrapper back to the request/notification the
// original handler expects. Registered per discovered handler type by ServiceCollectionExtensions.

internal sealed class CompatRequestHandlerAdapter<TRequest, TResponse>(MediatR.IRequestHandler<TRequest, TResponse> inner)
    : IRequestHandler<CompatRequestWrapper<TRequest, TResponse>, TResponse>
    where TRequest : MediatR.IRequest<TResponse>
{
    public Task<TResponse> Handle(CompatRequestWrapper<TRequest, TResponse> request, CancellationToken cancellationToken = default)
        => inner.Handle(request.Request, cancellationToken);
}

internal sealed class CompatCommandHandlerAdapter<TRequest>(MediatR.IRequestHandler<TRequest> inner)
    : IRequestHandler<CompatCommandWrapper<TRequest>>
    where TRequest : MediatR.IRequest
{
    public Task Handle(CompatCommandWrapper<TRequest> request, CancellationToken cancellationToken = default)
        => inner.Handle(request.Request, cancellationToken);
}

internal sealed class CompatNotificationHandlerAdapter<TNotification>(MediatR.INotificationHandler<TNotification> inner)
    : INotificationHandler<CompatNotificationWrapper<TNotification>>
    where TNotification : MediatR.INotification
{
    public Task Handle(CompatNotificationWrapper<TNotification> notification, CancellationToken cancellationToken = default)
        => inner.Handle(notification.Notification, cancellationToken);
}

internal sealed class CompatStreamRequestHandlerAdapter<TRequest, TResponse>(MediatR.IStreamRequestHandler<TRequest, TResponse> inner)
    : IStreamRequestHandler<CompatStreamRequestWrapper<TRequest, TResponse>, TResponse>
    where TRequest : MediatR.IStreamRequest<TResponse>
{
    public IAsyncEnumerable<TResponse> Handle(CompatStreamRequestWrapper<TRequest, TResponse> request, CancellationToken cancellationToken = default)
        => inner.Handle(request.Request, cancellationToken);
}
