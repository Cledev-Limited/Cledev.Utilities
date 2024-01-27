using Cledev.Core.Data;
using Newtonsoft.Json;
// ReSharper disable InconsistentNaming

namespace Cledev.Core.Domain;

public interface IAggregateRoot
{
    string Id { get; }
    int Version { get; }
    IEnumerable<IDomainEvent> UncommittedEvents { get; }
    void LoadFromHistory(IEnumerable<IDomainEvent> events);
}

public abstract class AggregateRoot : IAggregateRoot
{
    public string Id { get; protected set; }
    public int Version { get; private set; }
    
    [JsonIgnore]
    public IEnumerable<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();
    private readonly List<IDomainEvent> _uncommittedEvents = [];
    
    [JsonIgnore]
    public IEnumerable<IEntity> UncommittedEntities => _uncommittedEntities.AsReadOnly();
    protected readonly List<IEntity> _uncommittedEntities = [];
    
    protected AggregateRoot()
    {
        Id = Guid.NewGuid().ToString();
    }

    protected AggregateRoot(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
        }

        Id = id;
    }

    protected void AddEvent(IDomainEvent @event)
    {
        _uncommittedEvents.Add(@event);
        ApplyEvent(@event);
        AddEntities(@event);
    }
    
    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
            Version++;
        }
    }
    
    protected abstract void ApplyEvent<T>(T @event) where T : IDomainEvent;
    protected abstract void AddEntities<T>(T @event) where T : IDomainEvent;
}
