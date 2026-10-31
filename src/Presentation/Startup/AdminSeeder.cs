using Domain.Entities;
using Domain.Interfaces;

namespace Presentation.Startup
{
    public static class AdminSeeder
    {
        public static void Seed(IServiceProvider services, IConfiguration configuration)
        {
            var email = configuration["SeedAdmin:Email"];
            var password = configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            using var scope = services.CreateScope();

            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (userRepository.GetByEmail(email) is not null)
                return;

            userRepository.Add(new Administrator(email, passwordHasher.Hash(password)));
            userRepository.SaveChanges();
        }
    }
}
