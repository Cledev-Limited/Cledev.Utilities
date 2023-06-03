using Cledev.Core.Results;

namespace Cledev.Core.Queries;

internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapperBase<TResult> where TQuery : IQuery<TResult>
{
    public override async Task<Result<TResult>> Handle(IQuery<TResult> query, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handler = GetHandler<IQueryHandler<TQuery, TResult>>(serviceProvider);

        if (handler == null)
        {
            throw new Exception("No query handler found for query.");
        }

        return await handler.Handle((TQuery) query, cancellationToken);
    }
}