using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class UpdateItemHandler : IRequestHandler<UpdateItem>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateItemHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(UpdateItem command, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.SingleOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

        if (item is null)
        {
            return new Failure(ErrorCodes.NotFound, "Item", $"Item with id {command.Id} not found");
        }

        item.Update(command.Name, command.Description);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }
}