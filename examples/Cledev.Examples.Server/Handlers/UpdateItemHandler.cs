using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class UpdateItemHandler(ApplicationDbContext dbContext) : IRequestHandler<UpdateItem>
{
    public async Task<Result> Handle(UpdateItem command, CancellationToken cancellationToken)
    {
        var item = await dbContext.Items.SingleOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

        if (item is null)
        {
            return new Failure(ErrorCodes.NotFound, "Item", $"Item with id {command.Id} not found");
        }

        item.Update(command.Name, command.Description);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}