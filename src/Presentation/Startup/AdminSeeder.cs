using Domain.Entities;
using Domain.Interfaces;

namespace Presentation.Startup
{
    // Creates the first administrator when the app starts, because there is no endpoint
    // to create one without already being an administrator.
    // The credentials come from configuration (user-secrets / environment variables), never from the code.
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

            // Safe to run on every start: it does nothing if that email already exists.
            if (userRepository.GetByEmail(email) is not null)
                return;

            userRepository.Add(new Administrator(email, passwordHasher.Hash(password)));
            userRepository.SaveChanges();
        }
    }
}
