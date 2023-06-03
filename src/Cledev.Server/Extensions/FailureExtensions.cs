using Cledev.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace Cledev.Server.Extensions;

public static class FailureExtensions
{
    public static ActionResult ToActionResult(this Failure failure)
    {
        var (errorCode, title, description, type, _) = failure;

        var problemDetails = new ProblemDetails
        {
            Title = title ?? errorCode,
            Detail = description ?? errorCode,
            Type = type ?? errorCode,
            Status = errorCode.ToStatusCode()
        };

        return errorCode switch
        {
            ErrorCodes.NotFound => new NotFoundObjectResult(problemDetails),
            ErrorCodes.Unauthorized => new UnauthorizedObjectResult(problemDetails),
            _ => new UnprocessableEntityObjectResult(problemDetails)
        };
    }

    private static int ToStatusCode(this string errorCode)
    {
        return errorCode switch
        {
            ErrorCodes.NotFound => 404,
            ErrorCodes.Unauthorized => 401,
            ErrorCodes.Error => 422,
            _ => 400
        };
    }
}