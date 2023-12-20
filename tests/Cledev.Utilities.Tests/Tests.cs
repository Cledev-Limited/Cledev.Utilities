using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;

namespace Cledev.Utilities.Tests;

[TestFixture]
public class Tests
{
    [Test]
    public async Task Test()
    {
        var createTestItem = new CreateTestItem
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Item",
            Description = "Test Description"
        };
        
        await using var dbContext = new TestDbContext(Shared.CreateContextOptions());
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
            @event.Version.Should().Be(1);
        }
    }
}
