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

    public async Task<Result<TResult>> Process<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        if (query is null)
        {
            throw new ArgumentNullException(nameof(query));
        }
        
        var queryType = query.GetType();

        var handler = (QueryHandlerWrapperBase<TResult>)QueryHandlerWrappers.GetOrAdd(queryType, _ => 
            Activator.CreateInstance(typeof(QueryHandlerWrapper<,>).MakeGenericType(queryType, typeof(TResult))))!;

        var result = await handler.Handle(query, _serviceProvider, cancellationToken);

        return result;
    }
}
