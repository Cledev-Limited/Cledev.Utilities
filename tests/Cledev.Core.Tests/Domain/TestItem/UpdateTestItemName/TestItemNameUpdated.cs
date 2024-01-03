using Cledev.Core.Domain;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

public class TestItemNameUpdated : DomainEvent
{
    public string Name { get; init; } = null!;
}