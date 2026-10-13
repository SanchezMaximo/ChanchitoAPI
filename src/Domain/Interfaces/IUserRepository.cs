using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);

        User? GetByEmail(string email);

        Customer? GetCustomerById(Guid id);

        IReadOnlyList<Customer> GetAllCustomers();

        void SaveChanges();
    }
}