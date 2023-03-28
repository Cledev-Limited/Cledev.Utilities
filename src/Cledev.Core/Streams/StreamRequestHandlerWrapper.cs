namespace Cledev.Core.Streams;

internal class StreamRequestHandlerWrapper<TStreamRequest, TResponse> : StreamRequestHandlerWrapperBase<TResponse> where TStreamRequest : IStreamRequest<TResponse>
{
    public override IAsyncEnumerable<TResponse> Handle(IStreamRequest<TResponse> streamRequest, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var handler = GetHandler<IStreamRequestHandler<TStreamRequest, TResponse>>(serviceProvider);

        if (handler == null)
        {
            throw new Exception($"Handler not found for stream request of type {typeof(TStreamRequest)}");
        }

        return handler.Handle((TStreamRequest)streamRequest, cancellationToken);
    }
}