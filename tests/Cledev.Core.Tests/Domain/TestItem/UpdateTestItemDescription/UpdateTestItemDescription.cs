using Cledev.Core.Requests;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemDescription;

public class UpdateTestItemDescription : IRequest
{
    public string Id { get; init; } = null!;
    public string Description { get; init; } = null!;
}
