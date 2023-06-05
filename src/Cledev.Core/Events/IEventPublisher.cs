using Cledev.Core.Results;

namespace Cledev.Core.Events;

public interface IEventPublisher
{
    Task<IEnumerable<Result>> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
}
