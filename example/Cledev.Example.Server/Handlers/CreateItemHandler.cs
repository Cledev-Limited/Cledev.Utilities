using Cledev.Core.Commands;
using Cledev.Core.Results;
using Cledev.Example.Server.Data;
using Cledev.Example.Server.Data.Entities;
using Cledev.Example.Shared;

namespace Cledev.Example.Server.Handlers;

public class CreateItemHandler : ICommandHandler<CreateItem>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateItemHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(CreateItem command, CancellationToken cancellationToken)
    {
        var item = new Item(Guid.NewGuid(), command.Name!, command.Description!);

        _dbContext.Items.Add(item);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var itemCreated = new ItemCreated(item.Id, item.Name, item.Description);

        return new Success(itemCreated);
    }
}