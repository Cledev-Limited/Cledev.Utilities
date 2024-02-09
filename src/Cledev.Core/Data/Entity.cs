namespace Cledev.Core.Data;

public interface IEntity;

public class DbEntity<T>(T data, State state) where T : IEntity
{
    public T Data { get; } = data;
    public State State { get; } = state;
}

public enum State
{
    Added,
    Modified,
    Deleted
}
