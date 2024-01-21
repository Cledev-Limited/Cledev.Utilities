using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Cledev.Server.Caching;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class GetAllItemsHandler(ApplicationDbContext dbContext, ICacheManager cacheManager)
    : IRequestHandler<GetAllItems, GetAllItemsResponse>
{
    public async Task<Result<GetAllItemsResponse>> Handle(GetAllItems query, CancellationToken cancellationToken)
    {
        async Task<GetAllItemsResponse?> GetAllItemsAsync()
        {
            var items = await dbContext.Items.ToListAsync(cancellationToken);

            return new GetAllItemsResponse
            {
                Items = items.Select(item => new GetAllItemsResponse.Item(item.Id, item.Name, item.Description)).ToList()
            };
        }

        return (await cacheManager.GetOrSetAsync(cacheKey: "Items", cacheTime: TimeSpan.FromSeconds(60), acquireAsync: GetAllItemsAsync))!;
    }
}
