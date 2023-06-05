using Cledev.Core.Commands;
using Cledev.Core.Events;
using Cledev.Core.Queries;
using Cledev.Core.Results;
using Cledev.Core.Streams;

namespace Cledev.Core;

public interface IDispatcher
{
    Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    Task<Result<TResult>> Get<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Result>> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);
}
