using Domain.Entities;

namespace Domain.Interfaces
{
    public record TokenResult(string Token, DateTime ExpiresAt, string Role);

    public interface ITokenGenerator
    {
        TokenResult Generate(User user);
    }
}
