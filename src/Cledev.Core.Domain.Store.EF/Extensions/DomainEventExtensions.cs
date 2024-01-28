using Cledev.Core.Domain.Store.EF.Entities;
using Newtonsoft.Json;

namespace Cledev.Core.Domain.Store.EF.Extensions;

public static class DomainEventExtensions
{
    public static EventEntity ToEventEntity(this IDomainEvent @event)
    {
        return new EventEntity
        {
            Id = @event.Id,
            Type = @event.GetType().AssemblyQualifiedName ?? throw new InvalidOperationException(),
            Data = JsonConvert.SerializeObject(@event),
            TimeStamp = @event.TimeStamp,
            UserId = @event.UserId,
            Source = @event.Source
        };
    }
}
