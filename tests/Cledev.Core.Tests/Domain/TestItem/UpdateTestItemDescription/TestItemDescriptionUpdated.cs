using Cledev.Core.Domain;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemDescription;

public class TestItemDescriptionUpdated : DomainEvent
{
    public string Description { get; init; } = null!;
}
