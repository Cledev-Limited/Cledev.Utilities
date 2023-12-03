namespace Cledev.Core.Domain.Store.EF;

public class EFDomainStore : IDomainStore
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
