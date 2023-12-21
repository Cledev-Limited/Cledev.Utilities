using Cledev.Core.Domain;
using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;

namespace Cledev.Utilities.Tests;

public class TestItem : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

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
        }

        return true;
    }
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
        // var testItem = _testDbContext.Items.FirstOrDefault(i => i.Id == request.Id);
        var testItem = await _testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong);
        if (testItem.IsNotSuccess)
        {
            return testItem.Failure!;
        }
        testItem.Value!.UpdateName(request.Name);
        var result = await _testDbContext.SaveAggregate(testItem.Value!, expectedVersionNumber: testItem.Value!.Version, cancellationToken);
        return result;
    }
}
