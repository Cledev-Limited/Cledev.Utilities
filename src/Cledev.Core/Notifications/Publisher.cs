using Cledev.Core.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Core.Notifications;

public class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public Publisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<Result>> Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

        var tasks = handlers.Select(handler => handler.Handle(notification, cancellationToken)).ToList();

        return await Task.WhenAll(tasks);
    }
}
