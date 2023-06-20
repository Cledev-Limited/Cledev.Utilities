using Cledev.Core.Notifications;
using Cledev.Core.Results;
using Cledev.Examples.Shared;
using Cledev.Server.Caching;

namespace Cledev.Examples.Server.Handlers;

public class CacheHandlers : 
    INotificationHandler<ItemCreated>,
    INotificationHandler<ItemDeleted>,
    INotificationHandler<ItemUpdated>
{
    private readonly ICacheManager _cacheManager;

    public CacheHandlers(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<Result> Handle(ItemCreated @event, CancellationToken cancellationToken) => 
        await ClearItemsCache();

    public async Task<Result> Handle(ItemDeleted @event, CancellationToken cancellationToken) => 
        await ClearItemsCache();

    public async Task<Result> Handle(ItemUpdated @event, CancellationToken cancellationToken) => 
        await ClearItemsCache();

    private async Task<Result> ClearItemsCache()
    {
        await Task.CompletedTask;
        _cacheManager.Remove("Items");
        return Result.Ok();
    }
}
