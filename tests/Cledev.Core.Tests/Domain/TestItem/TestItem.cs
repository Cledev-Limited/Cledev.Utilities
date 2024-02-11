using Cledev.Core.Domain;
using Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;
using Cledev.Core.Tests.Domain.TestItem.CreateTestItem;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemDescription;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;
using Newtonsoft.Json;

namespace Cledev.Core.Tests.Domain.TestItem;

public partial class TestItem : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    [JsonProperty] private readonly List<TestSubItem> _subItems = [];
    [JsonIgnore] public IEnumerable<TestSubItem> SubItems => _subItems.AsReadOnly();

    public TestItem()
    {
    }

    public TestItem(string id, string name, string description)
    {
        AddEvent(new TestItemCreated
        {
            AggregateRootId = id,
            Name = name,
            Description = description
        });
    }

    public void UpdateName(string name)
    {
        AddEvent(new TestItemNameUpdated
        {
            AggregateRootId = Id,
            Name = name
        });
    }

    public void UpdateDescription(string description)
    {
        AddEvent(new TestItemDescriptionUpdated
        {
            AggregateRootId = Id,
            Description = description
        });
    }

    public void AddSubItem(string name)
    {
        AddEvent(new TestSubItemAdded
        {
            AggregateRootId = Id,
            SubItemId = Guid.NewGuid(),
            SubItemName = name
        });
    }
}
