using Cledev.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Core.Events;

public class EventPublisher : IEventPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public EventPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<Result> Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

        var enumerable = handlers as IEventHandler<TEvent>[] ?? handlers.ToArray();
        if (enumerable.Any() is false)
        {
            return Result.Ok();
        }
        
        var tasks = enumerable.Select(handler => handler.Handle(@event, cancellationToken)).ToList();

        var results = await Task.WhenAll(tasks);
        if (results.Any(result => result.IsFailure))
        {
            // TODO: Handle event publisher failed results
        }
        
        return Result.Ok();
    }
}
