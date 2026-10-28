using Domain.Enums;

namespace Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public TransactionType Type { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public Currency Currency { get; private set; }
        public bool Cancelled { get; private set; }

        // Rate (from the ExchangeRates table) in force when the transaction was made.
        // Only dollar transactions store it; it is null for pesos.
        public Guid? ExchangeRateId { get; private set; }

        private Transaction()
        {
        }

        public Transaction(TransactionType type, string description, decimal amount, DateTime date, Currency currency, Guid accountId, Guid? exchangeRateId = null)
        {
            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type), "The transaction type is not valid.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("The description is required.", nameof(description));

            ValidateAmount(amount);

            if (date > DateTime.UtcNow)
                throw new ArgumentOutOfRangeException(nameof(date), "The date cannot be in the future.");

            if (!Enum.IsDefined(currency))
                throw new ArgumentOutOfRangeException(nameof(currency), "The currency is not valid.");

            if (accountId == Guid.Empty)
                throw new ArgumentException("The account is required.", nameof(accountId));

            if (exchangeRateId == Guid.Empty)
                throw new ArgumentException("The exchange rate is not valid.", nameof(exchangeRateId));

            if (currency == Currency.USD && exchangeRateId is null)
                throw new ArgumentException("A dollar transaction must store the exchange rate of the moment.", nameof(exchangeRateId));

            Id = Guid.NewGuid();
            Type = type;
            Description = description;
            Amount = amount;
            Date = date;
            Currency = currency;
            AccountId = accountId;
            ExchangeRateId = currency == Currency.USD ? exchangeRateId : null;
            Cancelled = false;
        }

        public void Cancel()
        {
            if (Cancelled)
                throw new InvalidOperationException("The transaction is already cancelled.");
            
            Cancelled = true;
        }
        
        public void ChangeAmount(decimal newAmount)
        {
            if (Cancelled)
                throw new InvalidOperationException("A cancelled transaction cannot be modified.");
            ValidateAmount(newAmount);
            Amount = newAmount;
        }

        private static void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be greater than zero.");
        }
    }
}