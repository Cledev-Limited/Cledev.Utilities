using Cledev.Core.Commands;
using Cledev.Core.Results;
using Cledev.Example.Server.Data;
using Cledev.Example.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Example.Server.Handlers;

public class UpdateItemHandler : ICommandHandler<UpdateItem>
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

        var itemUpdated = new ItemUpdated(item.Id, item.Name, item.Description);

        return new Success(itemUpdated);
    }
}