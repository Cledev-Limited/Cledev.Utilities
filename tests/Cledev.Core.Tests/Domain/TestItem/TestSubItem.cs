namespace Cledev.Core.Tests.Domain.TestItem;

public class TestSubItem(Guid id, string name)
{
    public Guid Id { get; private set; } = id;
    public string Name { get; private set; } = name;
}
