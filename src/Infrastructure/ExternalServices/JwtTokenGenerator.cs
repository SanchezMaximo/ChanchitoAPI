using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.ExternalServices
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly JwtSettings settings;

        public JwtTokenGenerator(JwtSettings settings)
        {
            this.settings = settings;
        }

        public TokenResult Generate(User user)
        {
            var role = user is Administrator ? Roles.Administrator : Roles.Customer;
            var expiresAt = DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);

            return new TokenResult(token, expiresAt, role);
        }
    }
}
