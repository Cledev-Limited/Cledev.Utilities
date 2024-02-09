using Cledev.Core.Data;

namespace Cledev.Core.Tests.Data.Entities;

public class TestSubItemEntity : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string TestItemId { get; set; } = null!;
    public virtual TestItemEntity TestItem { get; set; } = null!;
}
