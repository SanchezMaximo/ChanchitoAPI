using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs
{
    public record TransactionResponse(Guid Id, TransactionType Type, string Description, decimal Amount, DateTime Date, Guid AccountId, Currency Currency, bool Cancelled, Guid? ExchangeRateId)
    {
        public static TransactionResponse From(Transaction transaction) =>
            new(transaction.Id, transaction.Type, transaction.Description, transaction.Amount, transaction.Date, transaction.AccountId, transaction.Currency, transaction.Cancelled, transaction.ExchangeRateId);
    }
}
