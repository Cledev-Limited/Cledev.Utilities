using Cledev.Core.Notifications;
using Cledev.Core.Results;
using Cledev.Examples.Shared;
using Cledev.Server.Caching;

namespace Cledev.Examples.Server.Handlers;

public class CacheHandlers(ICacheManager cacheManager) :
    INotificationHandler<ItemCreated>,
    INotificationHandler<ItemDeleted>,
    INotificationHandler<ItemUpdated>
{
    public async Task<Result> Handle(ItemCreated @event, CancellationToken cancellationToken) => 
        await ClearItemsCache();

    public async Task<Result> Handle(ItemDeleted @event, CancellationToken cancellationToken) => 
        await ClearItemsCache();

    public async Task<Result> Handle(ItemUpdated @event, CancellationToken cancellationToken) => 
        await ClearItemsCache();

    private async Task<Result> ClearItemsCache()
    {
        await Task.CompletedTask;
        cacheManager.Remove("Items");
        return Result.Ok();
    }
}
