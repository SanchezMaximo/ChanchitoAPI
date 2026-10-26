using Application.DTOs;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly ITokenGenerator tokenGenerator;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenGenerator tokenGenerator)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.tokenGenerator = tokenGenerator;
        }

        public LoginResponse Login(LoginRequest request)
        {
            var user = userRepository.GetByEmail(request.Email ?? string.Empty);

            // Same message for an unknown email and a wrong password, so emails cannot be guessed.
            if (user is null
                || string.IsNullOrEmpty(request.Password)
                || !passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            // The password is checked first, so the suspended status is only revealed to the real owner.
            if (!user.IsActive)
                throw new UserSuspendedException("This user is suspended and cannot log in.");

            var token = tokenGenerator.Generate(user);

            return new LoginResponse(token.Token, token.ExpiresAt, token.Role);
        }
    }
}
