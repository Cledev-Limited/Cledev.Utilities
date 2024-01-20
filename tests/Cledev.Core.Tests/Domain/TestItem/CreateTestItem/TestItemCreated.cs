using Cledev.Core.Domain;

namespace Cledev.Core.Tests.Domain.TestItem.CreateTestItem;

public class TestItemCreated : DomainEvent
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}
