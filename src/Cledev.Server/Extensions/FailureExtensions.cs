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
            ErrorCodes.UnprocessableEntity => new UnprocessableEntityObjectResult(problemDetails),
            ErrorCodes.BadRequest => new BadRequestObjectResult(problemDetails),
            _ => new UnprocessableEntityObjectResult(problemDetails)
        };
    }

    private static int ToStatusCode(this string errorCode)
    {
        return errorCode switch
        {
            ErrorCodes.NotFound => 404,
            ErrorCodes.Unauthorized => 401,
            ErrorCodes.UnprocessableEntity => 422,
            ErrorCodes.BadRequest => 400,
            _ => 422
        };
    }
}
