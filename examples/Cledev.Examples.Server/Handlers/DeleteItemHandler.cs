using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Shared;
using Microsoft.EntityFrameworkCore;

namespace Cledev.Examples.Server.Handlers;

public class DeleteItemHandler : IRequestHandler<DeleteItem>
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

        return Result.Ok();
    }
}