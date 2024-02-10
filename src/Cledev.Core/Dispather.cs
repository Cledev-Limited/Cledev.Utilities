using Cledev.Core.Notifications;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Streams;

namespace Cledev.Core;

public class Dispatcher(IRequestSender requestSender, IPublisher publisher, IStreamCreator streamCreator) : IDispatcher
{
    public async Task<Result> Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        return await requestSender.Send(request, cancellationToken);
    }

    public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return await requestSender.Send(request, cancellationToken);
    }

    public async Task<IEnumerable<Result>> Publish<TNotification>(INotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        return await publisher.Publish(notification, cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return streamCreator.Create(request, cancellationToken);
    }
}
