using Newtonsoft.Json;

namespace Cledev.Core.Domain.Store.EF.Entities;

public class EventEntity
{
    public string Id { get; set; } = null!;
    public string AggregateRootId { get; set; } = null!;
    public int Version { get; set; }
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
    public DateTimeOffset TimeStamp { get; set; }
    public string? UserId { get; set; }
    public string? Source { get; set; }
}

public static class DomainEventExtensions
{
    public static EventEntity ToEventEntity(IDomainEvent @event, int version)
    {
        return new EventEntity
        {
            Id = @event.Id,
            AggregateRootId = @event.AggregateRootId,
            // CommandId = @event.CommandId,
            Version = version,
            Type = @event.GetType().AssemblyQualifiedName ?? throw new InvalidOperationException(),
            Data = JsonConvert.SerializeObject(@event),
            TimeStamp = @event.TimeStamp,
            UserId = @event.UserId,
            Source = @event.Source
        };
    }
}
