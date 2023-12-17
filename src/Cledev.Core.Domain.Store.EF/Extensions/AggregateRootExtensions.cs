using Cledev.Core.Domain.Store.EF.Entities;
using Newtonsoft.Json;

namespace Cledev.Core.Domain.Store.EF.Extensions;

public static class AggregateRootExtensions
{
    public static AggregateEntity ToAggregateEntity(this IAggregateRoot aggregateRoot, int version)
    {
        return new AggregateEntity
        {
            Id = aggregateRoot.Id,
            Version = version,
            Type = aggregateRoot.GetType().AssemblyQualifiedName ?? throw new InvalidOperationException(),
            Data = JsonConvert.SerializeObject(aggregateRoot)
        };
    }
}
