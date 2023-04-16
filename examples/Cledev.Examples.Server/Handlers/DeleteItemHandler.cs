using Cledev.Core.Commands;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class DeleteItemHandler : ICommandHandler<DeleteItem>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteItemHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(DeleteItem command, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.SingleOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

        if (item is null)
        {
            return new Failure(ErrorCodes.NotFound, "Item", $"Item with id {command.Id} not found");
        }

        _dbContext.Items.Remove(item);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var itemDeleted = new ItemDeleted(item.Id);

        return new Success(itemDeleted);
    }
}