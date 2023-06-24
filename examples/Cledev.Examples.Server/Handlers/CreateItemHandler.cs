﻿using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Server.Data;
using Cledev.Examples.Server.Data.Entities;
using Cledev.Examples.Shared;

namespace Cledev.Examples.Server.Handlers;

public class CreateItemHandler : IRequestHandler<CreateItem>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateItemHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Result> Handle(CreateItem command, CancellationToken cancellationToken)
    {
        var item = new Item(Guid.NewGuid(), command.Name!, command.Description!);

        _dbContext.Items.Add(item);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
