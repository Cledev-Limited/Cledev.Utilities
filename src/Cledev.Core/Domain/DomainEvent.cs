namespace Cledev.Core.Domain;

public interface IDomainEvent
{
    string Id { get; set; }
    string AggregateRootId { get; set; }
}

public abstract class DomainEvent : IDomainEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string AggregateRootId { get; set; }
}
