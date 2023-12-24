// TODO: Move to http context extensions

using Cledev.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Cledev.Server.Services;

public interface IUserService
{
    string? GetCurrentIdentityUserId();
    string? GetCurrentIdentityUserEmail();
}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor) => 
        _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentIdentityUserId() =>
        IsUserAuthenticated() 
            ? _httpContextAccessor.HttpContext!.User.GetUserId() 
            : null;

    public string? GetCurrentIdentityUserEmail() => 
        IsUserAuthenticated() 
            ? _httpContextAccessor.HttpContext!.User.GetEmail() 
            : null;

    private bool IsUserAuthenticated()
    {
        var claimsPrincipal = _httpContextAccessor.HttpContext?.User;
        return claimsPrincipal?.Identity?.IsAuthenticated is true;
    }
}
