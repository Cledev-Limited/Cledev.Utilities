using AutoMapper;

namespace Cledev.Core.Mapping;

public class ObjectFactory(IMapperBase mapper) : IObjectFactory
{
    /// <inheritdoc />
    public dynamic CreateConcreteObject(object obj)
    {
        var type = obj.GetType();
        dynamic result = mapper.Map(obj, type, type);
        return result;
    }
}
