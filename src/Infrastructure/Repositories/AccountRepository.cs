using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext context;

        public AccountRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void Add(Account account) => context.Accounts.Add(account);

        public void AddTransaction(Transaction transaction) => context.Transactions.Add(transaction);

        public IReadOnlyList<Account> GetAll() =>
            context.Accounts
                .Include(a => a.Transactions)
                .ToList();

        public Account? GetById(Guid id) =>
            context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefault(a => a.Id == id);

        public void Remove(Account account) => context.Accounts.Remove(account);

        public void SaveChanges() => context.SaveChanges();
    }
}