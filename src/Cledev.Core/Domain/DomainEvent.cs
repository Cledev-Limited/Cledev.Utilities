namespace Cledev.Core.Domain;

public interface IDomainEvent
{
    string Id { get; set; }
    string AggregateRootId { get; set; }
    DateTimeOffset TimeStamp { get; set; }
    string? UserId { get; set; }
    string? Source { get; set; }
}

public abstract class DomainEvent : IDomainEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string AggregateRootId { get; set; }
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    public string? UserId { get; set; }
    public string? Source { get; set; }
}
