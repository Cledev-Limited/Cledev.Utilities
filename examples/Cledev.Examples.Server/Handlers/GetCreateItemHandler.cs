﻿using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Examples.Shared;

namespace Cledev.Examples.Server.Handlers;

public class GetCreateItemHandler : IRequestHandler<GetCreateItem, CreateItem>
{
    public async Task<Result<CreateItem>> Handle(GetCreateItem query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;

        return new CreateItem();
    }

    public async Task<Result<CreateItem>> HandleTestT(GetCreateItem query)
    {
        await Task.CompletedTask;

        return Result<CreateItem>.Ok(new CreateItem());
        return Result<CreateItem>.Fail(ErrorCodes.NotFound, "Item", "Item not found");

        return new Success<CreateItem>(new CreateItem());
        return new Failure(ErrorCodes.NotFound, "Item", "Item not found");

        return new CreateItem();
    }

    public async Task<Result> HandleTest(GetCreateItem query)
    {
        await Task.CompletedTask;

        return Result.Ok();
        return Result.Fail(ErrorCodes.NotFound, "Item", "Item not found");

        return new Success();
        return new Failure(ErrorCodes.NotFound, "Item", "Item not found");
    }
}