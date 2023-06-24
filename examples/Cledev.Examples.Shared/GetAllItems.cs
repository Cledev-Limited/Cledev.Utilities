using Cledev.Core.Requests;

namespace Cledev.Examples.Shared;

public record GetAllItems : IRequest<GetAllItemsResponse>;

public class GetAllItemsResponse
{
    public IList<Item> Items { get; set; } = new List<Item>();

    public record Item(Guid Id, string? Name, string? Description);
}