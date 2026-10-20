using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);

        User? GetByEmail(string email);

        Customer? GetCustomerById(Guid id);

        IReadOnlyList<Customer> GetAllCustomers();

        int CountCustomers();

        int CountActiveCustomers();

        void SaveChanges();
    }
}