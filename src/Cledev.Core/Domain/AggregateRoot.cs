

using Newtonsoft.Json;

namespace Cledev.Core.Domain;

public interface IAggregateRoot
{
    string Id { get; }
    int Version { get; }
    IEnumerable<IDomainEvent> UncommittedEvents { get; }
    void Apply(IEnumerable<IDomainEvent> events);
}

public abstract class AggregateRoot : IAggregateRoot
{
    public string Id { get; }
    public int Version { get; private set; }
    
    [JsonIgnore]
    public IEnumerable<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();
    private readonly List<IDomainEvent> _uncommittedEvents = new();
    
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
    }
    
    public void Apply(IEnumerable<IDomainEvent> domainEvents)
    {
        var events = domainEvents as IDomainEvent[] ?? domainEvents.ToArray();

        foreach (var @event in events)
        {
            Apply(@event);
        }
    }
    
    protected abstract bool Apply<T>(T @event) where T : IDomainEvent;
}
