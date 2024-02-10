using Cledev.Core.Domain;

namespace Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;

public class TestSubItemAdded : DomainEvent
{
    public Guid SubItemId { get; init; }
    public string SubItemName { get; init; } = null!;
}
