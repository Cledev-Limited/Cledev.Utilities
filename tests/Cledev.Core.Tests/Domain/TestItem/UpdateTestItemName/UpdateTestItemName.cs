using Cledev.Core.Requests;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

public class UpdateTestItemName : IRequest
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
}