namespace Cledev.Core.Domain;

public interface IRepository<T> where T : IAggregateRoot
{
    Task<T?> GetById(string id, int fromVersion = 1);
    Task Save(T aggregate);
}

public class Repository<T> : IRepository<T> where T : IAggregateRoot
{
    private readonly IEventStore _storeProvider;

    public Repository(IEventStore storeProvider)
    {
        _storeProvider = storeProvider;
    }

    public async Task<T?> GetById(string id, int fromVersion = 1)
    {
        var events = await _storeProvider.GetEvents(id, fromVersion);
        var domainEvents = events as DomainEvent[] ?? events.ToArray();
        if (!domainEvents.Any())
        {
            return default;
        }

        var aggregate = Activator.CreateInstance<T>();        
        aggregate.LoadsFromHistory(domainEvents);
        return aggregate;
    }
    
    public async Task Save(T aggregate)
    {
        await _storeProvider.AppendEvents(aggregate.Id, aggregate.UncommittedEvents);
    }
}
