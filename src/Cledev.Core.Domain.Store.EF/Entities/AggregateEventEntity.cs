namespace Cledev.Core.Domain.Store.EF.Entities;

public class AggregateEventEntity
{
    public string AggregateEntityId { get; set; } = null!;
    public string EventEntityId { get; set; } = null!;
    public int Sequence { get; set; }
    
    public virtual AggregateEntity AggregateEntity { get; set; } = null!;
    public virtual EventEntity EventEntity { get; set; } = null!;
}
