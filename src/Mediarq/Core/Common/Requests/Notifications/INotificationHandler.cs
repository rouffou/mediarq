namespace Mediarq.Core.Common.Requests.Notifications; 
public interface INotificationHandler<in TINotification> {

    Task Handle(INotification notification, CancellationToken cancellationToken = default);
}
