using System.Security.Claims;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.Extensions;

public static class ClaimPrincipalExtensions
{
    public static bool IsInRole(this ClaimsPrincipal principal, AdminRole role)
    {
        return principal.IsInRole(role.ToString());
    }
}