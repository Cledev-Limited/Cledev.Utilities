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

    public async Task<Result> Publish<TEvent>(TEvent? @event) where TEvent : IEvent
    {
        if (@event is null)
        {
            return Result.Fail(ErrorCodes.Error, title: "Null Argument", description: "Event is null");
        }

        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

        var tasks = handlers.Select(handler => handler.Handle(@event)).ToList();

        var results = await Task.WhenAll(tasks);
        if (results.Any(result => result.IsFailure))
        {
            // TODO: Handle event publisher failed results
        }
        
        return Result.Ok();
    }
}
