using Cledev.Core.Data;

namespace Cledev.Core.Tests.Data.Entities;

public sealed class TestSubItemEntity : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string TestItemId { get; set; } = null!;
    public TestItemEntity TestItem { get; set; } = null!;
}
