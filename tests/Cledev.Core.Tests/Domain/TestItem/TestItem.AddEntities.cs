using Cledev.Core.Data;
using Cledev.Core.Tests.Data.Entities;
using Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;
using Cledev.Core.Tests.Domain.TestItem.CreateTestItem;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemDescription;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

namespace Cledev.Core.Tests.Domain.TestItem;

public partial class TestItem
{
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
