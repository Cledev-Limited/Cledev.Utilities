namespace Cledev.Core.Domain.Store.EF.Entities;

public class EventEntity
{
    public string Id { get; set; } = null!;
    public int Sequence { get; set; }
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
    public DateTimeOffset TimeStamp { get; set; }
    public string? UserId { get; set; }
    public string? Source { get; set; }

    public string AggregateEntityId { get; set; } = null!;
    public virtual AggregateEntity AggregateEntity { get; set; } = null!;
}
