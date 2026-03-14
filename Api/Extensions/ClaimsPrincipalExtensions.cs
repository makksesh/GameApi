using System.Security.Claims;

namespace Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                    ?? user.FindFirst("sub")
                    ?? throw new InvalidOperationException("User ID claim not found.");

        return Guid.Parse(claim.Value);
    }

    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Role)?.Value
           ?? throw new InvalidOperationException("Role claim not found.");
}