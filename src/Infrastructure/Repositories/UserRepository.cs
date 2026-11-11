using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void Add(User user) => context.Users.Add(user);

        public User? GetByEmail(string email) =>
            context.Users.FirstOrDefault(u => u.Email == email);

        public Customer? GetCustomerById(Guid id) =>
            context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefault(c => c.Id == id);

        public IReadOnlyList<Customer> GetAllCustomers() =>
            context.Customers
                .Include(c => c.Accounts)
                .ToList();

        public IReadOnlyList<Administrator> GetAllAdministrators() =>
            context.Administrators.ToList();

        public User? GetById(Guid id) =>
            context.Users.FirstOrDefault(u => u.Id == id);

        public Administrator? GetAdministratorById(Guid id) =>
            context.Administrators.FirstOrDefault(a => a.Id == id);
            
        public int CountCustomers() => context.Customers.Count();

        public int CountActiveCustomers() => context.Customers.Count(c => c.IsActive);

        public void SaveChanges() => context.SaveChanges();
    }
}