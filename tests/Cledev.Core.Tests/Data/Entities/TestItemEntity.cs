using Cledev.Core.Data;

namespace Cledev.Core.Tests.Data.Entities;

public class TestItemEntity : IEntity
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public virtual List<TestSubItemEntity> TestSubItems { get; set; } = new();
}
