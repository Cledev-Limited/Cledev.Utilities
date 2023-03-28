namespace Cledev.Core.Events;

public interface IEvent
{
    DateTimeOffset TimeStamp { get; init; }
}
