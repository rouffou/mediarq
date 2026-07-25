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
    private const string PolymorphicJustification =
        "Only reached for notification types implementing IPolymorphicNotification: walks the base-type " +
        "hierarchy and builds closed INotificationHandler<TBase> service types via MakeGenericType.";

    // Cheap, trim-safe IsAssignableFrom check — always computed eagerly so the (common) non-polymorphic
    // path never touches the reflection-heavy members below.
    private static readonly bool IsPolymorphic = typeof(IPolymorphicNotification).IsAssignableFrom(typeof(TNotification));

    // Built lazily, once, only when IsPolymorphic is true — never referenced from a field initializer,
    // so the reflection-heavy [RequiresDynamicCode] path is never even reachable for the common case.
    [SuppressMessage("Major Code Smell", "S2743:A static field in a generic type is not shared among instances of different close constructed types",
        Justification = "Intentional: one cached array of base-type handler service types per closed TNotification type, not process-global state.")]
    private static Type[]? _polymorphicHandlerServiceTypes;

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

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = PolymorphicJustification)]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = PolymorphicJustification)]
    private static Task PublishCore(
        TNotification typedNotification,
        IHandlerResolver handlerResolver,
        INotificationPublisher notificationPublisher,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<INotificationHandler<TNotification>> handlers =
            handlerResolver.ResolveAll<INotificationHandler<TNotification>>();

        // Base-type handlers (IPolymorphicNotification opt-in only) — an empty array for every
        // notification type that doesn't opt in, so this never allocates on the common path.
        IReadOnlyList<INotificationHandler<TNotification>> polymorphicHandlers =
            IsPolymorphic ? ResolvePolymorphicHandlers(handlerResolver) : [];

        var concreteCount = handlers.Count;
        var polymorphicCount = polymorphicHandlers.Count;
        var count = concreteCount + polymorphicCount;

        if (count == 0)
        {
            // Publishing a notification with no registered handler is a no-op.
            return Task.CompletedTask;
        }

        if (count == 1)
        {
            // Single handler: ordering is moot, so skip the LINQ OrderBy and the List; a one-element
            // array still flows through the publisher (preserving any decorator, e.g. diagnostics).
            var only = concreteCount == 1 ? handlers[0] : polymorphicHandlers[0];
            var single = new Func<CancellationToken, Task>[] { ct => only.Handle(typedNotification, ct) };
            return notificationPublisher.Publish(single, cancellationToken);
        }

        // Handlers implementing IOrderedNotificationHandler run by ascending Order; others keep their
        // registration order (stable sort) and run after, defaulting to int.MaxValue. The OrderBy is
        // only paid for when at least one handler actually opts into ordering — otherwise registration
        // order is already correct and the LINQ allocation is avoided. Absent any ordering, concrete-type
        // handlers run first, then base-type handlers from most to least specific — the natural iteration
        // order below.
        var hasOrdered = false;
        for (var i = 0; i < concreteCount; i++)
        {
            if (handlers[i] is IOrderedNotificationHandler)
            {
                hasOrdered = true;
                break;
            }
        }

        if (!hasOrdered)
        {
            for (var i = 0; i < polymorphicCount; i++)
            {
                if (polymorphicHandlers[i] is IOrderedNotificationHandler)
                {
                    hasOrdered = true;
                    break;
                }
            }
        }

        // A fixed-size array (sized to the known handler count) avoids the List wrapper object.
        var callbacks = new Func<CancellationToken, Task>[count];
        var index = 0;
        if (hasOrdered)
        {
            IEnumerable<INotificationHandler<TNotification>> allHandlers =
                polymorphicCount == 0 ? handlers : handlers.Concat(polymorphicHandlers);
            foreach (var handler in allHandlers.OrderBy(h => h is IOrderedNotificationHandler ordered ? ordered.Order : int.MaxValue))
            {
                var captured = handler;
                callbacks[index++] = ct => captured.Handle(typedNotification, ct);
            }
        }
        else
        {
            for (var i = 0; i < concreteCount; i++)
            {
                var captured = handlers[i];
                callbacks[index++] = ct => captured.Handle(typedNotification, ct);
            }

            for (var i = 0; i < polymorphicCount; i++)
            {
                var captured = polymorphicHandlers[i];
                callbacks[index++] = ct => captured.Handle(typedNotification, ct);
            }
        }

        // The configured INotificationPublisher decides how handlers are invoked (parallel, sequential, ...).
        return notificationPublisher.Publish(callbacks, cancellationToken);
    }

    [RequiresUnreferencedCode(PolymorphicJustification)]
    [RequiresDynamicCode(PolymorphicJustification)]
    private static List<INotificationHandler<TNotification>> ResolvePolymorphicHandlers(IHandlerResolver handlerResolver)
    {
        var serviceTypes = _polymorphicHandlerServiceTypes ??= BuildPolymorphicHandlerServiceTypes();
        if (serviceTypes.Length == 0)
        {
            return [];
        }

        List<INotificationHandler<TNotification>> resolved = [];
        foreach (var serviceType in serviceTypes)
        {
            foreach (var handler in handlerResolver.ResolveAll(serviceType))
            {
                // Safe without a cast exception: serviceType is INotificationHandler<TBase> for a TBase
                // that TNotification derives from, and INotificationHandler<in TNotification> is
                // contravariant, so every resolved instance already satisfies INotificationHandler<TNotification>.
                resolved.Add((INotificationHandler<TNotification>)handler);
            }
        }

        return resolved;
    }

    [RequiresUnreferencedCode(PolymorphicJustification)]
    [RequiresDynamicCode(PolymorphicJustification)]
    private static Type[] BuildPolymorphicHandlerServiceTypes()
    {
        List<Type>? serviceTypes = null;
        var baseType = typeof(TNotification).BaseType;
        while (baseType is not null && baseType != typeof(object))
        {
            if (typeof(INotification).IsAssignableFrom(baseType))
            {
                serviceTypes ??= [];
                serviceTypes.Add(typeof(INotificationHandler<>).MakeGenericType(baseType));
            }

            baseType = baseType.BaseType;
        }

        return serviceTypes?.ToArray() ?? [];
    }
}
