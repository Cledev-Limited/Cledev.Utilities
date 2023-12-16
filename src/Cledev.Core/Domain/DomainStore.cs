namespace Cledev.Core.Domain;

public interface IDomainStore
{
    Task<IEnumerable<IDomainEvent>> GetEvents(string id, int fromVersion = 1);
    // Task AppendEvents(string id, IEnumerable<IDomainEvent> events);
    Task<IAggregateRoot> Get(string id);
    Task Save(IAggregateRoot aggregateRoot);
}

public class DefaultDomainStore : IDomainStore
{
    public Task<IEnumerable<IDomainEvent>> GetEvents(string id, int fromVersion = 1)
    {
        throw new NotImplementedException();
    }

    // public Task AppendEvents(string id, IEnumerable<IDomainEvent> events) 
    // {
    //     throw new NotImplementedException();
    // }

    public Task<IAggregateRoot> Get(string id)
    {
        throw new NotImplementedException();
    }

    public Task Save(IAggregateRoot aggregateRoot)
    {
        throw new NotImplementedException();
    }
}
