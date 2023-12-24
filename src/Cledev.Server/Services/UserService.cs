using Cledev.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Cledev.Server.Services;

public interface IUserService
{
    string? GetCurrentIdentityUserId();
    string? GetCurrentIdentityUserEmail();
    bool UserIsAuthenticated();
}

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor) => 
        _httpContextAccessor = httpContextAccessor;

    public string? GetCurrentIdentityUserId() =>
        _httpContextAccessor.CurrentUserId();

    public string? GetCurrentIdentityUserEmail() =>
        _httpContextAccessor.CurrentUserEmail();

    public bool UserIsAuthenticated() => 
        _httpContextAccessor.UserIsAuthenticated();
}
