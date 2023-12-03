namespace Cledev.Core.Domain;

public interface IAggregateRoot
{
    string Id { get; }
    int Version { get; }
    IEnumerable<IDomainEvent> UncommittedEvents { get; }
    void LoadsFromHistory(IEnumerable<IDomainEvent> events);
}

public abstract class AggregateRoot : IAggregateRoot
{
    public string Id { get; }
    public int Version { get; private set; }

    private readonly List<IDomainEvent> _uncommittedEvents = new();
    public IEnumerable<IDomainEvent> UncommittedEvents => _uncommittedEvents.AsReadOnly();

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

    public void LoadsFromHistory(IEnumerable<IDomainEvent> domainEvents)
    {
        var events = domainEvents as IDomainEvent[] ?? domainEvents.ToArray();

        foreach (var @event in events)
        {
            Apply(@event);
        }
    }
    
    protected void AddEvent(IDomainEvent @event)
    {
        _uncommittedEvents.Add(@event);
        Apply(@event);
    }
    
    protected abstract bool Apply<T>(T @event) where T : IDomainEvent;
}
