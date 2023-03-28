namespace Cledev.Core.Events;

public abstract record EventBase : IEvent
{
    public DateTimeOffset TimeStamp { get; init; } = DateTimeOffset.UtcNow;
}
