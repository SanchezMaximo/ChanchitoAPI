using Domain.Entities;

namespace Application.DTOs
{
    public record AccountResponse(Guid Id, string Name, decimal Balance, string Type, string Currency, Guid CustomerId)
    {
        public static AccountResponse From(Account account) =>
            new(account.Id, account.Name, account.Balance, account.Type.ToString(), account.Currency.ToString(), account.CustomerId);
    }
}