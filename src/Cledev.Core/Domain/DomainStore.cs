namespace Cledev.Core.Domain;

public interface IDomainStore
{
    Task<IEnumerable<IDomainEvent>> GetEvents(string id, int fromVersion = 1);
    Task AppendEvents(string id, IEnumerable<IDomainEvent> events);
}

public class DefaultDomainStore : IDomainStore
{
    public Task<IEnumerable<IDomainEvent>> GetEvents(string id, int fromVersion = 1)
    {
        throw new NotImplementedException();
    }

    public Task AppendEvents(string id, IEnumerable<IDomainEvent> events) 
    {
        throw new NotImplementedException();
    }
}
