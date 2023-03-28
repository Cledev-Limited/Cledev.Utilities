using Cledev.Core.Extensions;
using FluentValidation.Results;

namespace Cledev.Core.Results;

public record Failure(string ErrorCode = ErrorCodes.Error, string? Title = null, string? Description = null, string? Type = null);

public static class FailureExtensions
{
    public static Failure WithTitle(this Failure failure, string title) => 
        failure with { Title = title };

    public static Failure WithDescription(this Failure failure, string description) => 
        failure with { Description = description };

    public static Failure WithDescription(this Failure failure, string description, IEnumerable<string> items) => 
        failure with { Description = $"{description}: {string.Join(", ", items)}" };

    public static Failure WithDescription(this Failure failure, ValidationResult validationResult) => 
        failure with { Description = validationResult.Errors.ToErrorMessage() };

    public static Failure WithType(this Failure failure, string type) =>
        failure with { Type = type };
}

public static class ErrorCodes
{
    public const string Error = "Error";
    public const string NotFound = "NotFound";
    public const string Unauthorized = "Unauthorized";
}
