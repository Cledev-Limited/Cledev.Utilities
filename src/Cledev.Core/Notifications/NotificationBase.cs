namespace Cledev.Core.Notifications;

public record NotificationBase : INotification
{
    public DateTimeOffset TimeStamp { get; init; } = DateTimeOffset.UtcNow;
}