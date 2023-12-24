using Microsoft.AspNetCore.Http;

namespace Cledev.Core.Extensions;

public static class HttpContextAccessorExtensions
{
    public static string? CurrentUserId(this IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.UserIsAuthenticated() 
            ? httpContextAccessor.HttpContext?.User.GetUserId() 
            : null;

    public static string? CurrentUserEmail(this IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.UserIsAuthenticated() 
            ? httpContextAccessor.HttpContext?.User.GetEmail() 
            : null;
    
    public static bool UserIsAuthenticated(this IHttpContextAccessor httpContextAccessor)
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;
        return claimsPrincipal?.Identity?.IsAuthenticated is true;
    }
}
