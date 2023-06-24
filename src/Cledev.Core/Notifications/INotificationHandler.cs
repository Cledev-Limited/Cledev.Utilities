using Cledev.Core.Results;

namespace Cledev.Core.Notifications;

public interface INotificationHandler<in TNotification> where TNotification : INotification
{
    Task<Result> Handle(TNotification notification, CancellationToken cancellationToken = default);
}
