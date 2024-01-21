using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Server.Data.Entities;
using Cledev.Examples.Shared;

namespace Cledev.Examples.Server.Handlers;

public class CreateItemHandler(ApplicationDbContext dbContext) : IRequestHandler<CreateItem>
{
    public async Task<Result> Handle(CreateItem command, CancellationToken cancellationToken)
    {
        var item = new Item(Guid.NewGuid(), command.Name!, command.Description!);

        dbContext.Items.Add(item);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
