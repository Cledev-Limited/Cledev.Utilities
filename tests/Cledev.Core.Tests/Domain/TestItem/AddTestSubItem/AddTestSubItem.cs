using Cledev.Core.Requests;

namespace Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;

public class AddTestSubItem : IRequest
{
    public string Id { get; init; } = null!;
    public string SubItemName { get; init; } = null!;
}