using Cledev.Core.Queries;
using Cledev.Core.Requests;

namespace Cledev.Examples.Shared;

public record GetItem(Guid Id) : IRequest<GetItemResponse>;

public record GetItemResponse(Guid Id, string? Name, string? Description);