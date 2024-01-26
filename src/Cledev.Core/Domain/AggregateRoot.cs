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
    public IEnumerable<IEntity> UncommittedReadModels => _uncommittedReadModels.AsReadOnly();
    protected readonly List<IEntity> _uncommittedReadModels = [];
    
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
        Apply(@event);
        AddReadModels(@event);
    }
    
    public void LoadFromHistory(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            Apply(@event);
            Version++;
        }
    }
    
    protected abstract void Apply<T>(T @event) where T : IDomainEvent;
    protected abstract void AddReadModels<T>(T @event) where T : IDomainEvent;
}
