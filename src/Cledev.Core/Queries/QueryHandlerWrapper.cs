using Cledev.Core.Results;

namespace Cledev.Core.Queries;

internal class QueryHandlerWrapper<TQuery, TResult> : QueryHandlerWrapperBase<TResult> where TQuery : IQuery<TResult>
{
    public override async Task<Result<TResult>> Handle(IQuery<TResult> query, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handler = GetHandler<IQueryHandler<TQuery, TResult>>(serviceProvider);

        if (handler == null)
        {
            return Result<TResult>.Fail(title: "Handler not found", description: $"Handler not found for query of type {typeof(TQuery)}");
        }

        return await handler.Handle((TQuery) query, cancellationToken);
    }
}