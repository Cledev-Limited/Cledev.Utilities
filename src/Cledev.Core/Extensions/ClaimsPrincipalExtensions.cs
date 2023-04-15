using System.Security.Claims;

namespace Cledev.Core.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Identities.First().Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Identities.First().Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
}
