using Cledev.Core.Results;

namespace Cledev.Core.Queries;

public interface IQueryProcessor
{
    Task<Result<TResult>> Process<TResult>(IQuery<TResult>? query);
}
