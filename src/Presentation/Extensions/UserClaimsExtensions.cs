using System.Security.Claims;

namespace Presentation.Extensions
{
    public static class UserClaimsExtensions
    {
        // Id of the logged-in user, taken from the JWT. Guid.Empty if the token has no valid id.
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }
}
