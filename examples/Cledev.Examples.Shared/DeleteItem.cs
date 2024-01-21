using Cledev.Core.Notifications;
using Cledev.Core.Requests;

namespace Cledev.Examples.Shared;

public class DeleteItem(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}

public record ItemDeleted(Guid Id) : NotificationBase;