using System.Collections.Concurrent;

namespace Cledev.Core.Streams;

public class StreamCreator : IStreamCreator
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, object?> StreamHandlerWrappers = new();

    public StreamCreator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAsyncEnumerable<TResponse> Create<TResponse>(IStreamRequest<TResponse>? request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        
        var streamRequestType = request.GetType();
        var handler = (StreamRequestHandlerWrapperBase<TResponse>)StreamHandlerWrappers.GetOrAdd(streamRequestType,
            t => Activator.CreateInstance(typeof(StreamRequestHandlerWrapper<,>).MakeGenericType(streamRequestType, typeof(TResponse))))!;

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }
}
