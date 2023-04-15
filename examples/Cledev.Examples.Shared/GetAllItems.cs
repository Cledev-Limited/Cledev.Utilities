using Cledev.Core.Queries;

namespace Cledev.Examples.Shared;

public record GetAllItems : QueryBase<GetAllItemsResponse>;

public class GetAllItemsResponse
{
    public IList<Item> Items { get; set; } = new List<Item>();

    public record Item(Guid Id, string? Name, string? Description);
}