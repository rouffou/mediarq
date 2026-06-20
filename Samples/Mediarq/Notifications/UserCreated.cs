using Mediarq.Core.Common.Requests.Notifications;

namespace Mediarq.Samples.Notifications;

/// <summary>Raised after a user is created; handled by zero or more handlers.</summary>
public record UserCreated(Guid UserId, string UserName) : INotification;

/// <summary>Logs that a user was created (one of several notification handlers).</summary>
public sealed class LogUserCreatedHandler(ILogger<LogUserCreatedHandler> logger) : INotificationHandler<UserCreated>
{
    public Task Handle(UserCreated notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("User {UserId} created: {UserName}", notification.UserId, notification.UserName);
        return Task.CompletedTask;
    }
}

/// <summary>Simulates queuing a welcome email (another notification handler).</summary>
public sealed class SendWelcomeEmailHandler(ILogger<SendWelcomeEmailHandler> logger) : INotificationHandler<UserCreated>
{
    public Task Handle(UserCreated notification, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Welcome email queued for {UserName}", notification.UserName);
        return Task.CompletedTask;
    }
}
