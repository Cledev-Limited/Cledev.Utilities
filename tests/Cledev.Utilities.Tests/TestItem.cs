using Cledev.Core.Data;
using Cledev.Core.Domain;
using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;

namespace Cledev.Utilities.Tests;

public class TestItem : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public IEnumerable<TestSubItem> SubItems => _subItems.AsReadOnly();
    private readonly List<TestSubItem> _subItems = new();
    
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
    
    protected override bool Apply<T>(T @event)
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

        return true;
    }

    protected override bool AddDataEntities<T>(T @event)
    {
        switch (@event)
        {
            case TestItemCreated itemCreated:
                DataEntities.Add(new TestItemEntity
                {
                    Id = itemCreated.AggregateRootId,
                    Name = itemCreated.Name,
                    Description = itemCreated.Description,
                    State = State.Added
                });
                break;
            case TestItemNameUpdated itemNameUpdated:
                DataEntities.Add(new TestItemEntity
                {
                    Id = itemNameUpdated.AggregateRootId,
                    Name = itemNameUpdated.Name,
                    Description = Description,
                    State = State.Modified
                });
                break;
            case TestItemDescriptionUpdated itemDescriptionUpdated:
                DataEntities.Add(new TestItemEntity
                {
                    Id = itemDescriptionUpdated.AggregateRootId,
                    Name = Name,
                    Description = itemDescriptionUpdated.Description,
                    State = State.Modified
                });                
                break;
            case TestSubItemAdded subItemAdded:
                DataEntities.Add(new TestSubItemEntity
                {
                    Id = subItemAdded.SubItemId,
                    Name = subItemAdded.SubItemName,
                    TestItemId = subItemAdded.AggregateRootId,
                    State = State.Added
                });
                break;
        }

        return true;
    }
}

public class TestSubItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class CreateTestItem : IRequest
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public class UpdateTestItemName : IRequest
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
}

public class UpdateTestItemDescription : IRequest
{
    public string Id { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public class AddTestSubItem : IRequest
{
    public string Id { get; init; } = null!;
    public string SubItemName { get; init; } = null!;
}

public class TestItemCreated : DomainEvent
{
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}

public class TestItemNameUpdated : DomainEvent
{
    public string Name { get; init; } = null!;
}

public class TestItemDescriptionUpdated : DomainEvent
{
    public string Description { get; init; } = null!;
}

public class TestSubItemAdded : DomainEvent
{
    public Guid SubItemId { get; set; }
    public string SubItemName { get; init; } = null!;
}

public class CreateTestItemHandler : IRequestHandler<CreateTestItem>
{
    private readonly TestDbContext _testDbContext;

    public CreateTestItemHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Result> Handle(CreateTestItem request, CancellationToken cancellationToken = default)
    {
        var testItem = new TestItem(request.Id, request.Name, request.Description);
        var result = await _testDbContext.SaveAggregate(testItem, expectedVersionNumber: 0, cancellationToken);
        return result;
    }
}

public class UpdateTestItemNameHandler : IRequestHandler<UpdateTestItemName>
{
    private readonly TestDbContext _testDbContext;

    public UpdateTestItemNameHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Result> Handle(UpdateTestItemName request, CancellationToken cancellationToken = default)
    {
        var testItem = await _testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong);
        if (testItem.IsNotSuccess)
        {
            return testItem.Failure!;
        }
        var expectedVersionNumber = testItem.Value!.Version;
        testItem.Value!.UpdateName(request.Name);
        var result = await _testDbContext.SaveAggregate(testItem.Value!, expectedVersionNumber, cancellationToken);
        return result;
    }
}

public class AddTestSubItemHandler : IRequestHandler<AddTestSubItem>
{
    private readonly TestDbContext _testDbContext;

    public AddTestSubItemHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Result> Handle(AddTestSubItem request, CancellationToken cancellationToken = default)
    {
        var testItem = await _testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong);
        if (testItem.IsNotSuccess)
        {
            return testItem.Failure!;
        }
        var expectedVersionNumber = testItem.Value!.Version;
        testItem.Value!.AddSubItem(request.SubItemName);
        var result = await _testDbContext.SaveAggregate(testItem.Value!, expectedVersionNumber, cancellationToken);
        return result;
    }
}

public class TestItemEntity : Entity
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public virtual List<TestSubItemEntity> TestSubItems { get; set; } = new();
}

public class TestSubItemEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string TestItemId { get; set; } = null!;
    public virtual TestItemEntity TestItem { get; set; } = null!;
}
