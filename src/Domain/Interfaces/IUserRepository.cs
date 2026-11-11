using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        void Add(User user);

        User? GetByEmail(string email);

        Customer? GetCustomerById(Guid id);

        IReadOnlyList<Customer> GetAllCustomers();

        IReadOnlyList<Administrator> GetAllAdministrators();
        User? GetById(Guid id);

        int CountCustomers();

        int CountActiveCustomers();

        void SaveChanges();
    }
}