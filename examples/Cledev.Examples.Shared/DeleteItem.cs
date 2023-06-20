﻿using Cledev.Core.Events;
using Cledev.Core.Requests;

namespace Cledev.Examples.Shared;

public class DeleteItem : IRequest
{
    public Guid Id { get; }

    public DeleteItem(Guid id)
    {
        Id = id;
    }
}

public record ItemDeleted(Guid Id) : EventBase;