using Cledev.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Cledev.Server.Services;

public interface IUserService
{
    string? GetCurrentIdentityUserId();
    string? GetCurrentIdentityUserEmail();
    bool UserIsAuthenticated();
}

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    public string? GetCurrentIdentityUserId() =>
        httpContextAccessor.GetCurrentUserId();

    public string? GetCurrentIdentityUserEmail() =>
        httpContextAccessor.GetCurrentUserEmail();

    public bool UserIsAuthenticated() => 
        httpContextAccessor.UserIsAuthenticated();
}
