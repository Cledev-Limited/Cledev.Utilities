using Cledev.Core.Notifications;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Streams;

namespace Cledev.Core;

public interface IDispatcher
{
    Task<Result> Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;
    Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task<IEnumerable<Result>> Publish<TNotification>(INotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);
}
