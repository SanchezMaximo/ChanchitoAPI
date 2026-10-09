using Domain.Entities;
using Domain.Interfaces;

public class AccountService
{
    private readonly IAccountRepository accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        this.accountRepository = accountRepository;
    }

    public Transaction RegisterIncome(Guid accountId, decimal amount, string description, DateTime date)
    {
        var account = accountRepository.GetById(accountId)
            ?? throw new KeyNotFoundException($"No existe una cuenta con el id {accountId}.");

        var transaction = account.RegisterIncome(amount, description, date);

        accountRepository.AddTransaction(transaction);
        accountRepository.SaveChanges();

        return transaction;
    }
}