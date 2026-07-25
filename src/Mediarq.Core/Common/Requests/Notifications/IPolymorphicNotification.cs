namespace Mediarq.Core.Common.Requests.Notifications;

/// <summary>
/// Opt-in: marks a notification whose publish also dispatches to <see cref="INotificationHandler{TNotification}"/>
/// implementations registered for any base type in its class hierarchy — not just its own concrete type.
/// </summary>
/// <remarks>
/// Without this marker, publishing resolves handlers for the notification's exact concrete type only
/// (the default, and the reflection-free fast path). A notification implementing this interface also
/// resolves <c>INotificationHandler&lt;TBase&gt;</c> for every ancestor class (walking <c>BaseType</c>,
/// stopping at <see cref="object"/>) that itself implements <see cref="INotification"/> — via
/// <see cref="Mediarq.Core.Common.Resolvers.IHandlerResolver.ResolveAll(System.Type)"/>, so this path is
/// not reflection-free; it is only paid for notification types that opt in.
/// </remarks>
/// <example>
/// <code>
/// public abstract record DomainEvent : INotification;
/// public sealed record OrderPlaced(Guid OrderId) : DomainEvent, IPolymorphicNotification;
///
/// // Receives every DomainEvent, including OrderPlaced and any other derived type:
/// public sealed class AuditLogHandler : INotificationHandler&lt;DomainEvent&gt; { /* ... */ }
/// </code>
/// </example>
public interface IPolymorphicNotification : INotification;
