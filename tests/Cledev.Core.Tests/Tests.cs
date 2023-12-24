using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Cledev.Core.Tests;

public class Tests
{
    [Fact]
    public async Task ShouldCreateNewAggregateAndEvents()
    {
        var createTestItem = new CreateTestItem
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Item",
            Description = "Test Description"
        };
        
        await using var dbContext = new TestDbContext(Shared.CreateContextOptions(), Shared.CreateHttpContextAccessor());
        var handler = new CreateTestItemHandler(dbContext);
        await handler.Handle(createTestItem);

        var testItem = dbContext.Items.FirstOrDefault(i => i.Id == createTestItem.Id);
        var aggregate = dbContext.Aggregates.FirstOrDefault(a => a.Id == createTestItem.Id);
        var @event = dbContext.Events.FirstOrDefault(a => a.AggregateRootId == createTestItem.Id);

        using (new AssertionScope())
        {
            testItem.Should().NotBeNull();
            testItem!.Name.Should().Be(createTestItem.Name);
            testItem.Description.Should().Be(createTestItem.Description);
            
            aggregate.Should().NotBeNull();
            aggregate!.Type.Should().Be(typeof(TestItem).AssemblyQualifiedName);
            aggregate.Version.Should().Be(1);
        
            @event.Should().NotBeNull();
            @event!.Type.Should().Be(typeof(TestItemCreated).AssemblyQualifiedName);
            @event.Sequence.Should().Be(1);
        }
    }
    
    [Fact]
    public async Task ShouldUpdateAggregateAndAddNewEvents()
    {
        var createTestItem = new CreateTestItem
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Item",
            Description = "Test Description"
        };

        await using (var dbContext1 = new TestDbContext(Shared.CreateContextOptions(), Shared.CreateHttpContextAccessor()))
        {
            var createTestItemHandler = new CreateTestItemHandler(dbContext1);
            await createTestItemHandler.Handle(createTestItem);
        }
        
        var updateTestItemName = new UpdateTestItemName
        {
            Id = createTestItem.Id,
            Name = "Updated Test Item"
        };

        await using var dbContext2 = new TestDbContext(Shared.CreateContextOptions(), Shared.CreateHttpContextAccessor());
        var updateTestItemHandler = new UpdateTestItemNameHandler(dbContext2);
        await updateTestItemHandler.Handle(updateTestItemName);

        var testItem = dbContext2.Items.FirstOrDefault(i => i.Id == createTestItem.Id);
        var aggregate = dbContext2.Aggregates.FirstOrDefault(a => a.Id == createTestItem.Id);
        var @event = dbContext2.Events.LastOrDefault(a => a.AggregateRootId == createTestItem.Id);

        using (new AssertionScope())
        {
            testItem.Should().NotBeNull();
            testItem!.Name.Should().Be(updateTestItemName.Name);
            testItem.Description.Should().Be(createTestItem.Description);
            
            aggregate.Should().NotBeNull();
            aggregate!.Type.Should().Be(typeof(TestItem).AssemblyQualifiedName);
            aggregate.Version.Should().Be(2);
        
            @event.Should().NotBeNull();
            @event!.Type.Should().Be(typeof(TestItemNameUpdated).AssemblyQualifiedName);
            @event.Sequence.Should().Be(2);
        }
    }
    
    [Fact]
    public async Task ShouldAddSubItem()
    {
        var createTestItem = new CreateTestItem
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Item",
            Description = "Test Description"
        };

        await using (var dbContext1 = new TestDbContext(Shared.CreateContextOptions(), Shared.CreateHttpContextAccessor()))
        {
            var createTestItemHandler = new CreateTestItemHandler(dbContext1);
            await createTestItemHandler.Handle(createTestItem);
        }
        
        var addTestSubItem = new AddTestSubItem
        {
            Id = createTestItem.Id,
            SubItemName = "Test Sub Item"
        };

        await using var dbContext2 = new TestDbContext(Shared.CreateContextOptions(), Shared.CreateHttpContextAccessor());
        var addTestSubItemHandler = new AddTestSubItemHandler(dbContext2);
        await addTestSubItemHandler.Handle(addTestSubItem);

        var testSubItem = dbContext2.SubItems.FirstOrDefault(i => i.TestItemId == createTestItem.Id);
        var aggregate = dbContext2.Aggregates.FirstOrDefault(a => a.Id == createTestItem.Id);
        var @event = dbContext2.Events.LastOrDefault(a => a.AggregateRootId == createTestItem.Id);

        using (new AssertionScope())
        {
            testSubItem.Should().NotBeNull();
            testSubItem!.Name.Should().Be(addTestSubItem.SubItemName);
            
            aggregate.Should().NotBeNull();
            aggregate!.Type.Should().Be(typeof(TestItem).AssemblyQualifiedName);
            aggregate.Version.Should().Be(2);
        
            @event.Should().NotBeNull();
            @event!.Type.Should().Be(typeof(TestSubItemAdded).AssemblyQualifiedName);
            @event.Sequence.Should().Be(2);
        }
    }
}
