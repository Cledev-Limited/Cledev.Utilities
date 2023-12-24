namespace Cledev.Core.Domain;

public interface IRepository<T> where T : IAggregateRoot
{
    Task<T?> GetById(string id, int fromVersion = 1);
    Task Save(T aggregate);
}

public class Repository<T> : IRepository<T> where T : IAggregateRoot
{
    private readonly IDomainStore _domainStore;

    public Repository(IDomainStore domainStore)
    {
        _domainStore = domainStore;
    }

    public async Task<T?> GetById(string id, int fromVersion = 1)
    {
        // TODO: Add options for weak and strong views
        
        var events = await _domainStore.GetEvents(id, fromVersion);
        var domainEvents = events as DomainEvent[] ?? events.ToArray();
        if (!domainEvents.Any())
        {
            return default;
        }

        var aggregate = Activator.CreateInstance<T>();        
        aggregate.LoadFromHistory(domainEvents);
        return aggregate;
    }
    
    public async Task Save(T aggregate)
    {
        await _domainStore.Save(aggregate);
    }
}
