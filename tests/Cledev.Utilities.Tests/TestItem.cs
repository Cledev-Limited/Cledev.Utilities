using Cledev.Core.Domain;

namespace Cledev.Utilities.Tests;

public class TestItem : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private TestItem() { }

    public TestItem(string id, string name, string description)
    {
        AddEvent(new ItemCreated
        {
            AggregateRootId = id,
            Name = name,
            Description = description
        });
    }

    public void UpdateName(string name)
    {
        AddEvent(new ItemNameUpdated
        {
            AggregateRootId = Id,
            Name = name
        });
    }
    
    public void UpdateDescription(string description)
    {
        AddEvent(new ItemDescriptionUpdated
        {
            AggregateRootId = Id,
            Description = description
        });
    }
    
    protected override bool Apply<T>(T @event)
    {
        switch (@event)
        {
            case ItemCreated itemCreated:
                Id = itemCreated.AggregateRootId;
                Name = itemCreated.Name;
                Description = itemCreated.Description;
                break;
            case ItemNameUpdated itemNameUpdated:
                Name = itemNameUpdated.Name;
                break;
            case ItemDescriptionUpdated itemDescriptionUpdated:
                Description = itemDescriptionUpdated.Description;
                break;
        }

        return true;
    }
}

public class ItemCreated : DomainEvent
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public class ItemNameUpdated : DomainEvent
{
    public string Name { get; init; } = null!;
}

public class ItemDescriptionUpdated : DomainEvent
{
    public string Description { get; init; } = null!;
}
