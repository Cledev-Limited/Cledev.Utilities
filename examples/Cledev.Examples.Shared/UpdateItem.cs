using Cledev.Core.Notifications;
using Cledev.Core.Requests;
using FluentValidation;

namespace Cledev.Examples.Shared;

public record GetUpdateItem(Guid Id) : IRequest<UpdateItem>;

public class UpdateItem : IRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public class UpdateItemValidator : AbstractValidator<UpdateItem>
{
    public UpdateItemValidator(IUpdateItemValidationRules updateItemValidationRules)
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .Length(1, 50).WithMessage("Item name must be at least 1 and at max 50 characters long.");

        RuleFor(command => command.Name)
            .MustAsync(ItemNameBeUnique).WithMessage(command => $"An item with name {command.Name} already exists.")
            .When(command => string.IsNullOrEmpty(command.Name) is false);

        RuleFor(command => command.Description)
            .NotEmpty().WithMessage("Item description is required.")
            .Length(1, 50).WithMessage("Item description must be at least 1 and at max 50 characters long.");

        async Task<bool> ItemNameBeUnique(UpdateItem command, string name, CancellationToken cancellation) =>
            await updateItemValidationRules.IsItemNameUnique(command.Id, name);
    }
}

public interface IUpdateItemValidationRules
{
    Task<bool> IsItemNameUnique(Guid id, string name);
}

public record ItemUpdated(Guid Id, string Name, string Description) : NotificationBase;