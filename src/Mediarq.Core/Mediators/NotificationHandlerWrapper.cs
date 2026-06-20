using System.Diagnostics.CodeAnalysis;
using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Common.Requests.Validators;
using Mediarq.Core.Common.Resolvers;

namespace Mediarq.Core.Mediators;

/// <summary>
/// Non-generic entry point the <see cref="Mediator"/> calls to publish a notification. An abstract class
/// (rather than an interface) so the call dispatches through a vtable slot. Implementations are cached
/// per concrete notification type.
/// </summary>
[SuppressMessage("Major Code Smell", "S1694:An abstract class should have both abstract and concrete methods",
    Justification = "Intentionally an abstract class, not an interface: vtable dispatch is cheaper than interface dispatch on the publish hot path.")]
internal abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(
        object notification,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken);
}

/// <summary>
/// Closed over the concrete notification type. Because <typeparamref name="TNotification"/> is known,
/// handler resolution and invocation are strongly-typed — no <c>GetMethod</c>/<c>Invoke</c> reflection
/// on the publish path.
/// </summary>
/// <typeparam name="TNotification">The concrete notification type.</typeparam>
internal sealed class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{
    public override Task Handle(
        object notification,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken)
    {
        var typedNotification = (TNotification)notification;

        // Validate the notification first when a validator is registered for it. A notification has no
        // return value, so an invalid one (a programming error) is reported by throwing rather than being
        // published. The common case (no validator) stays on the synchronous fast path below.
        var validators = handlerResolver.ResolveAll<IValidator<TNotification>>();
        if (validators is { Count: > 0 })
        {
            return ValidateThenPublish(typedNotification, validators, handlerResolver, notificationPublisher, cancellationToken);
        }

        return PublishCore(typedNotification, handlerResolver, notificationPublisher, cancellationToken);
    }

    private static async Task ValidateThenPublish(
        TNotification notification,
        IReadOnlyList<IValidator<TNotification>> validators,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken)
    {
        List<ValidationPropertyError>? failures = null;
        for (var i = 0; i < validators.Count; i++)
        {
            var results = await validators[i].ValidateAsync(notification, cancellationToken).ConfigureAwait(false);
            foreach (var result in results)
            {
                if (!result.IsValid)
                {
                    failures ??= [];
                    failures.AddRange(result.Errors);
                }
            }
        }

        if (failures is { Count: > 0 })
        {
            throw new NotificationValidationException(typeof(TNotification), failures);
        }

        await PublishCore(notification, handlerResolver, notificationPublisher, cancellationToken).ConfigureAwait(false);
    }

    private static Task PublishCore(
        TNotification typedNotification,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<INotificationHandler<TNotification>> handlers =
            handlerResolver.ResolveAll<INotificationHandler<TNotification>>();

        var count = handlers.Count;
        if (count == 0)
        {
            // Publishing a notification with no registered handler is a no-op.
            return Task.CompletedTask;
        }

        if (count == 1)
        {
            // Single handler: ordering is moot, so skip the LINQ OrderBy and the List; a one-element
            // array still flows through the publisher (preserving any decorator, e.g. diagnostics).
            var only = handlers[0];
            var single = new Func<CancellationToken, Task>[] { ct => only.Handle(typedNotification, ct) };
            return notificationPublisher.Publish(single, cancellationToken);
        }

        // Handlers implementing IOrderedNotificationHandler run by ascending Order; others keep their
        // registration order (stable sort) and run after, defaulting to int.MaxValue. The OrderBy is
        // only paid for when at least one handler actually opts into ordering — otherwise registration
        // order is already correct and the LINQ allocation is avoided.
        IEnumerable<INotificationHandler<TNotification>> orderedHandlers = handlers;
        for (var i = 0; i < count; i++)
        {
            if (handlers[i] is IOrderedNotificationHandler)
            {
                orderedHandlers = handlers.OrderBy(h => h is IOrderedNotificationHandler ordered ? ordered.Order : int.MaxValue);
                break;
            }
        }

        // A fixed-size array (sized to the known handler count) avoids the List wrapper object.
        var callbacks = new Func<CancellationToken, Task>[count];
        var index = 0;
        foreach (var handler in orderedHandlers)
        {
            var captured = handler;
            callbacks[index++] = ct => captured.Handle(typedNotification, ct);
        }

        // The configured INotificationPublisher decides how handlers are invoked (parallel, sequential, ...).
        return notificationPublisher.Publish(callbacks, cancellationToken);
    }
}
