using System.Reflection;
using AutoMapper;
using Cledev.Core.Events;
using Cledev.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCledevCore(this IServiceCollection services, params Type[] types)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var typeList = types.ToList();
        typeList.Add(typeof(ServiceCollectionExtensions));

        services.Scan(s => s
            .FromAssembliesOf(typeList)
            .AddClasses(classes => classes.NotInNamespaceOf(typeof(Result)))
            .AsImplementedInterfaces());

        services.AddAutoMapper(typeList);

        return services;
    }

    private static IServiceCollection AddAutoMapper(this IServiceCollection services, IEnumerable<Type> types)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.ShouldMapMethod = (m => false);
            foreach (var type in types)
            {
                var typesToMap = type.Assembly.GetTypes()
                    .Where(t =>
                        t.GetTypeInfo().IsClass &&
                        !t.GetTypeInfo().IsAbstract &&
                        typeof(IEvent).IsAssignableFrom(t))
                    .ToList();

                foreach (var typeToMap in typesToMap)
                {
                    configuration.CreateMap(typeToMap, typeToMap);
                }
            }
        });

        services.AddSingleton(_ => mapperConfiguration.CreateMapper());

        return services;
    }
}