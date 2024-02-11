using Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;
using Cledev.Core.Tests.Domain.TestItem.CreateTestItem;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemDescription;
using Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

namespace Cledev.Core.Tests.Domain.TestItem;

public partial class TestItem
{
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
}
