using Cledev.Core.Results;

namespace Cledev.Core.Events;

public interface IEventPublisher
{
    Task<Result> Publish<TEvent>(TEvent? @event) where TEvent : IEvent;
}
