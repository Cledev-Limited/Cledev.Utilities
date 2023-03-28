using Cledev.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Core.Queries;

internal abstract class QueryHandlerWrapperBase<TResult>
{
    protected static THandler? GetHandler<THandler>(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<THandler>();
    }

    public abstract Task<Result<TResult>> Handle(IQuery<TResult> query, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}