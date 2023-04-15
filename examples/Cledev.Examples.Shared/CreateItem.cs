using Cledev.Core.Commands;
using Cledev.Core.Events;
using Cledev.Core.Queries;
using FluentValidation;

namespace Cledev.Examples.Shared;

public record GetCreateItem : QueryBase<CreateItem>;

public class CreateItem : CommandBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CreateItemValidator : AbstractValidator<CreateItem>
{
    public CreateItemValidator(ICreateItemValidationRules createItemValidationRules)
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

        async Task<bool> ItemNameBeUnique(CreateItem command, string? name, CancellationToken cancellation) =>
            await createItemValidationRules.IsItemNameUnique(name!);
    }
}

public interface ICreateItemValidationRules
{
    Task<bool> IsItemNameUnique(string name);
}

public record ItemCreated(Guid Id, string Name, string Description) : EventBase;