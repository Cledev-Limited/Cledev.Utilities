using Cledev.Core.Events;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Streams;

namespace Cledev.Core;

public interface IDispatcher
{
    // Task<Result> Send<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;
    Task<Result> Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
    // Task<Result<TResult>> Get<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
    Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Result>> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent;
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);
}
