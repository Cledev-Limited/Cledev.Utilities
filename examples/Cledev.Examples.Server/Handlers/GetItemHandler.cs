﻿using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Cledev.Server.Caching;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class GetItemHandler(ApplicationDbContext dbContext, ICacheManager cacheManager)
    : IRequestHandler<GetItem, GetItemResponse>
{
    private readonly ICacheManager _cacheManager = cacheManager;

    public async Task<Result<GetItemResponse>> Handle(GetItem query, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items.SingleOrDefaultAsync(item => item.Id == query.Id, cancellationToken);

        if (item is null)
        {
            return new Failure(ErrorCodes.NotFound, "Item", $"Item with id {query.Id} not found");
        }

        return new GetItemResponse(item.Id, item.Name, item.Description);
    }
}