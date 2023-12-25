using System.ComponentModel.DataAnnotations.Schema;

namespace Cledev.Core.Data;

public interface IEntity
{
    State State { get; set; }
}

public abstract class Entity : IEntity
{
    [NotMapped]
    public State State { get; set; }
}

public enum State
{
    Added,
    Modified,
    Deleted
}
