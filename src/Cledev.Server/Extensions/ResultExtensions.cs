using System.Diagnostics;
using Cledev.Core.Results;
using Microsoft.AspNetCore.Mvc;

namespace Cledev.Server.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result)
    {
        return result.Match(
            _ => new OkObjectResult(null),
            failure => failure.ToActionResult()
        );
    }

    public static ActionResult ToActionResult<TResult>(this Result<TResult> result)
    {
        return result.Match(
            success => new OkObjectResult(success.Result),
            failure => failure.ToActionResult()
        );
    }
    
    public static void UpdateActivityIfNeeded(this Result result)
    {
        if (result.IsSuccess || result.IsFailure && result.Failure?.Tags?.Any() is false)
        {
            return;
        }

        foreach (var tag in result.Failure!.Tags!)
        {
            Activity.Current?.AddTag(tag.Key, tag.Value);
        }
    }
    
    public static void UpdateActivityIfNeeded<TResult>(this Result<TResult> result)
    {
        if (result.IsSuccess || result.IsFailure && result.Failure?.Tags?.Any() is false)
        {
            return;
        }

        foreach (var tag in result.Failure!.Tags!)
        {
            Activity.Current?.AddTag(tag.Key, tag.Value);
        }
    }
}
