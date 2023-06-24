using System.Collections.Concurrent;
using Cledev.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Core.Requests;

public class RequestSender : IRequestSender
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, object?> RequestHandlerWrappers = new();
    
    public RequestSender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest>>();

        if (handler is null)
        {
            throw new Exception("Request handler not found.");
        }

        return await handler.Handle(request, cancellationToken);
    }

    public async Task<Result<TResponse>> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }
        
        var requestType = request.GetType();

        var handler = (RequestHandlerWrapperBase<TResponse>)RequestHandlerWrappers.GetOrAdd(requestType, _ => 
            Activator.CreateInstance(typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, typeof(TResponse))))!;

        var result = await handler.Handle(request, _serviceProvider, cancellationToken);

        return result;
    }
}
