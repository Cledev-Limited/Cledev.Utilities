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
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

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
    
    protected override void ApplyEvent<T>(T @event)
    {
        switch (@event)
        {
            case TestItemCreated itemCreated:
                ApplyEvent(itemCreated);
                break;
            case TestItemNameUpdated itemNameUpdated:
                ApplyEvent(itemNameUpdated);
                break;
            case TestItemDescriptionUpdated itemDescriptionUpdated:
                ApplyEvent(itemDescriptionUpdated);
                break;
            case TestSubItemAdded subItemAdded:
                ApplyEvent(subItemAdded);
                break;
        }
    }

    private void ApplyEvent(TestItemCreated itemCreated)
    {
        Id = itemCreated.AggregateRootId;
        Name = itemCreated.Name;
        Description = itemCreated.Description;
    }

    private void ApplyEvent(TestItemNameUpdated itemNameUpdated)
    {
        Name = itemNameUpdated.Name;
    }
    
    private void ApplyEvent(TestItemDescriptionUpdated itemDescriptionUpdated)
    {
        Description = itemDescriptionUpdated.Description;
    }

    private void ApplyEvent(TestSubItemAdded subItemAdded)
    {
        _subItems.Add(new TestSubItem(subItemAdded.SubItemId, subItemAdded.SubItemName));
    }
    
    protected override void AddEntities<T>(T @event)
    {
        switch (@event)
        {
            case TestItemCreated itemCreated:
                AddEntities(itemCreated);
                break;
            case TestItemNameUpdated itemNameUpdated:
                AddEntities(itemNameUpdated);
                break;
            case TestItemDescriptionUpdated itemDescriptionUpdated:
                AddEntities(itemDescriptionUpdated);
                break;
            case TestSubItemAdded subItemAdded:
                AddEntities(subItemAdded);
                break;
        }
    }

    private void AddEntities(TestItemCreated itemCreated)
    {
        _uncommittedEntities.Add(new DbEntity<IEntity>(new TestItemEntity
        {
            Id = itemCreated.AggregateRootId,
            Name = itemCreated.Name,
            Description = itemCreated.Description
        }, State.Added));
    }

    private void AddEntities(TestItemNameUpdated itemNameUpdated)
    {
        _uncommittedEntities.Add(new DbEntity<IEntity>(new TestItemEntity
        {
            Id = itemNameUpdated.AggregateRootId,
            Name = itemNameUpdated.Name,
            Description = Description,
        }, State.Modified));
    }

    private void AddEntities(TestItemDescriptionUpdated itemDescriptionUpdated)
    {
        _uncommittedEntities.Add(new DbEntity<IEntity>(new TestItemEntity
        {
            Id = itemDescriptionUpdated.AggregateRootId,
            Name = Name,
            Description = itemDescriptionUpdated.Description,
        }, State.Modified));
    }

    private void AddEntities(TestSubItemAdded subItemAdded)
    {
        _uncommittedEntities.Add(new DbEntity<IEntity>(new TestSubItemEntity
        {
            Id = subItemAdded.SubItemId,
            Name = subItemAdded.SubItemName,
            TestItemId = subItemAdded.AggregateRootId
        }, State.Added));
    }
}
