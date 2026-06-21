using Mediarq.Core.Common.Exceptions;

namespace Mediarq.Core.Common.Requests.Validators;

/// <summary>
/// Thrown when a notification fails validation before it is published. Unlike requests — whose
/// validation failures are surfaced as a failed <c>Result</c> — a notification has no return value, so
/// an invalid notification (a programming error) is reported by throwing this exception. The individual
/// property failures are available on <see cref="Errors"/>.
/// </summary>
public sealed class NotificationValidationException : MediarqException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationValidationException"/> class.
    /// </summary>
    /// <param name="notificationType">The notification type that failed validation.</param>
    /// <param name="errors">The property-level validation failures.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="notificationType"/> or <paramref name="errors"/> is <see langword="null"/>.</exception>
    public NotificationValidationException(Type notificationType, IReadOnlyList<ValidationPropertyError> errors)
        : base(BuildMessage(notificationType, errors))
    {
        ArgumentNullException.ThrowIfNull(notificationType);
        ArgumentNullException.ThrowIfNull(errors);

        NotificationType = notificationType;
        Errors = errors;
    }

    /// <summary>Gets the notification type that failed validation.</summary>
    public Type NotificationType { get; }

    /// <summary>Gets the property-level validation failures that caused the notification to be rejected.</summary>
    public IReadOnlyList<ValidationPropertyError> Errors { get; }

    private static string BuildMessage(Type notificationType, IReadOnlyList<ValidationPropertyError> errors)
    {
        ArgumentNullException.ThrowIfNull(notificationType);
        ArgumentNullException.ThrowIfNull(errors);

        return $"Validation failed for notification '{notificationType.Name}' with {errors.Count} error(s): "
            + string.Join("; ", errors);
    }
}
