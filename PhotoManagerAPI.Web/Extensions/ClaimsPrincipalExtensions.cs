using System.Security.Claims;

namespace PhotoManagerAPI.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        new Ulid(Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty)).ToGuid();
}