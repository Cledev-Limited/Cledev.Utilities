using Cledev.Core.Results;

namespace Cledev.Core.Events;

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task<Result> Handle(TEvent @event, CancellationToken cancellationToken);
}
