namespace Cledev.Examples.Server.Data.Entities;

public class Item
{
    public Guid Id { get; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public Item() { }

    public Item(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }
}