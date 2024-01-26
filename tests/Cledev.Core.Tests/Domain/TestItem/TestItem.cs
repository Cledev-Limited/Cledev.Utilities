using Cledev.Core.Data;
using Cledev.Core.Domain;
using Cledev.Core.Tests.Data.Entities;
using Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;
using Cledev.Core.Tests.Domain.TestItem.CreateTestItem;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemDescription;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

namespace Cledev.Core.Tests.Domain.TestItem;

public class TestItem : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public IEnumerable<TestSubItem> SubItems => _subItems.AsReadOnly();
    private readonly List<TestSubItem> _subItems = [];
    
    public TestItem() { }

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
    
    protected override void Apply<T>(T @event)
    {
        switch (@event)
        {
            case TestItemCreated itemCreated:
                Id = itemCreated.AggregateRootId;
                Name = itemCreated.Name;
                Description = itemCreated.Description;
                break;
            case TestItemNameUpdated itemNameUpdated:
                Name = itemNameUpdated.Name;
                break;
            case TestItemDescriptionUpdated itemDescriptionUpdated:
                Description = itemDescriptionUpdated.Description;
                break;
            case TestSubItemAdded subItemAdded:
                _subItems.Add(new TestSubItem
                {
                    Id = subItemAdded.SubItemId,
                    Name = subItemAdded.SubItemName
                });
                break;
        }
    }

    protected override void AddReadModels<T>(T @event)
    {
        switch (@event)
        {
            case TestItemCreated itemCreated:
                _uncommittedReadModels.Add(new TestItemEntity
                {
                    Id = itemCreated.AggregateRootId,
                    Name = itemCreated.Name,
                    Description = itemCreated.Description,
                    State = State.Added
                });
                break;
            case TestItemNameUpdated itemNameUpdated:
                _uncommittedReadModels.Add(new TestItemEntity
                {
                    Id = itemNameUpdated.AggregateRootId,
                    Name = itemNameUpdated.Name,
                    Description = Description,
                    State = State.Modified
                });
                break;
            case TestItemDescriptionUpdated itemDescriptionUpdated:
                _uncommittedReadModels.Add(new TestItemEntity
                {
                    Id = itemDescriptionUpdated.AggregateRootId,
                    Name = Name,
                    Description = itemDescriptionUpdated.Description,
                    State = State.Modified
                });                
                break;
            case TestSubItemAdded subItemAdded:
                _uncommittedReadModels.Add(new TestSubItemEntity
                {
                    Id = subItemAdded.SubItemId,
                    Name = subItemAdded.SubItemName,
                    TestItemId = subItemAdded.AggregateRootId,
                    State = State.Added
                });
                break;
        }
    }
}
