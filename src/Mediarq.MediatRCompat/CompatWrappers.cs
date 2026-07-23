using Mediarq.Core.Common.Requests.Abstraction;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Streaming;

namespace Mediarq.MediatRCompat;

// Bridges a MediatR-shaped request/notification into the Mediarq marker interface its own dispatch
// pipeline requires (ICommandOrQuery<TResponse> / ICommand / INotification / IStreamRequest<TResponse>).
// Composition, not inheritance: the original MediatR type is untouched, so existing request/handler
// classes need no code changes to flow through Mediarq's ISender/IPublisher.

internal sealed record CompatRequestWrapper<TRequest, TResponse>(TRequest Request) : ICommandOrQuery<TResponse>
    where TRequest : MediatR.IRequest<TResponse>;

internal sealed record CompatCommandWrapper<TRequest>(TRequest Request) : ICommand
    where TRequest : MediatR.IRequest;

internal sealed record CompatNotificationWrapper<TNotification>(TNotification Notification) : INotification
    where TNotification : MediatR.INotification;

internal sealed record CompatStreamRequestWrapper<TRequest, TResponse>(TRequest Request) : IStreamRequest<TResponse>
    where TRequest : MediatR.IStreamRequest<TResponse>;
