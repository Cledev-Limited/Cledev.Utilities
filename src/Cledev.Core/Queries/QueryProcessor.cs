using System.Collections.Concurrent;
using Cledev.Core.Results;

namespace Cledev.Core.Queries;

public class QueryProcessor : IQueryProcessor
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, object?> QueryHandlerWrappers = new();

    public QueryProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result<TResult>> Process<TResult>(IQuery<TResult>? query)
    {
        if (query is null)
        {
            return Result<TResult>.Fail(ErrorCodes.Error, title: "Null Argument", description: $"Query of type {typeof(IQuery<TResult>)} is null");
        }

        var queryType = query.GetType();

        var handler = (QueryHandlerWrapperBase<TResult>)QueryHandlerWrappers.GetOrAdd(queryType,
            t => Activator.CreateInstance(typeof(QueryHandlerWrapper<,>).MakeGenericType(queryType, typeof(TResult))))!;

        var result = await handler.Handle(query, _serviceProvider);

        return result;
    }
}
