using System.Security.Claims;

namespace Presentation.Extensions
{
    public static class UserClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }
}
