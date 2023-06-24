using Cledev.Core.Notifications;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Streams;

namespace Cledev.Core;

public class Dispatcher : IDispatcher
{
    private readonly IRequestSender _requestSender;
    private readonly IPublisher _publisher;
    private readonly IStreamCreator _streamCreator;

    public Dispatcher(IRequestSender requestSender, IPublisher publisher, IStreamCreator streamCreator)
    {
        _requestSender = requestSender;
        _publisher = publisher;
        _streamCreator = streamCreator;
    }

    public async Task<Result> Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        return await _requestSender.Send(request, cancellationToken);
    }

    public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return await _requestSender.Send(request, cancellationToken);
    }

    public async Task<IEnumerable<Result>> Publish<TNotification>(INotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        return await _publisher.Publish(notification, cancellationToken);
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return _streamCreator.Create(request, cancellationToken);
    }
}
