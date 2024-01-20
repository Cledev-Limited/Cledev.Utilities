using Cledev.Core.Requests;

namespace Cledev.Core.Tests.Domain.TestItem.CreateTestItem;

public class CreateTestItem : IRequest
{
    public string Id { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
}
