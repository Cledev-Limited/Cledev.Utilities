namespace Cledev.Core.Domain.Store.EF.Entities;

public class AggregateEntity : AuditableEntity
{
    public string Id { get; set; } = null!;
    public int Version { get; set; }
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
}
