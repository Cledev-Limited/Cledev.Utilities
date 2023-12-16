using Newtonsoft.Json;

namespace Cledev.Core.Domain.Store.EF.Entities;

public class AggregateEntity
{
    public string Id { get; set; } = null!;
    public int Version { get; set; }
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
    public DateTimeOffset CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset LastUpdatedDate { get; set; }
    public string? LastUpdatedBy { get; set; }
}

public static class AggregateRootExtensions
{
    public static AggregateEntity ToAggregateEntity(IAggregateRoot aggregateRoot, int version)
    {
        return new AggregateEntity
        {
            Id = aggregateRoot.Id,
            Version = version,
            Type = aggregateRoot.GetType().AssemblyQualifiedName ?? throw new InvalidOperationException(),
            Data = JsonConvert.SerializeObject(aggregateRoot),
            // TimeStamp = aggregateRoot.TimeStamp,
            // UserId = aggregateRoot.UserId
        };
    }
}
