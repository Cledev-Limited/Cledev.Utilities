using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Cledev.Server.Caching;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class GetAllItemsHandler : IRequestHandler<GetAllItems, GetAllItemsResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheManager _cacheManager;

    public GetAllItemsHandler(ApplicationDbContext dbContext, ICacheManager cacheManager)
    {
        _dbContext = dbContext;
        _cacheManager = cacheManager;
    }

    public async Task<Result<GetAllItemsResponse>> Handle(GetAllItems query, CancellationToken cancellationToken)
    {
        async Task<GetAllItemsResponse?> GetAllItemsAsync()
        {
            var items = await _dbContext.Items.ToListAsync(cancellationToken);

            return new GetAllItemsResponse
            {
                Items = items.Select(item => new GetAllItemsResponse.Item(item.Id, item.Name, item.Description)).ToList()
            };
        }

        return (await _cacheManager.GetOrSetAsync(cacheKey: "Items", cacheTime: TimeSpan.FromSeconds(60), acquireAsync: GetAllItemsAsync))!;
    }
}
