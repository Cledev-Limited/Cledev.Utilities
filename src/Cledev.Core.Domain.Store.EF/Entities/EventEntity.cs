namespace Cledev.Core.Domain.Store.EF.Entities;

public class EventEntity
{
    public string Id { get; set; } = null!;
    public string AggregateRootId { get; set; } = null!;
    public int Sequence { get; set; }
    public string Type { get; set; } = null!;
    public string Data { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public string? UserId { get; set; }
    public string? Source { get; set; }
}
