using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAccountRepository
    {
        void Add(Account account);

        void AddTransaction(Transaction transaction);

        IReadOnlyList<Account> GetAll();

        Account? GetById(Guid id);

        void Remove(Account account);

        void SaveChanges();
    }
}