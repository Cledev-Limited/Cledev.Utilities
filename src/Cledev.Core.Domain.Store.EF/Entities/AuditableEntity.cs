namespace Cledev.Core.Domain.Store.EF.Entities;

public interface IAuditableEntity
{
    DateTimeOffset CreatedDate { get; set; }
    string? CreatedBy { get; set; }
    DateTimeOffset LastUpdatedDate { get; set; }
    string? LastUpdatedBy { get; set; }
}

public abstract class AuditableEntity : IAuditableEntity
{
    public DateTimeOffset CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset LastUpdatedDate { get; set; }
    public string? LastUpdatedBy { get; set; }
}
