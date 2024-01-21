using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class GetUpdateItemHandler(ApplicationDbContext dbContext) : IRequestHandler<GetUpdateItem, UpdateItem>
{
    public async Task<Result<UpdateItem>> Handle(GetUpdateItem query, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items.SingleOrDefaultAsync(item => item.Id == query.Id, cancellationToken);

        if (item is null)
        {
            return new Failure(ErrorCodes.NotFound, "Item", $"Item with id {query.Id} not found");
        }

        return new UpdateItem
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description
        };
    }
}