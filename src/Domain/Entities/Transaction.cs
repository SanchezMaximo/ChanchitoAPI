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

        private Transaction()
        {
        }

        public Transaction(TransactionType type, string description, decimal amount, DateTime date, Currency currency, Guid accountId)
        {
            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type), "El tipo de movimiento no es válido.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("La descripción es obligatoria.", nameof(description));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "El monto debe ser mayor a cero.");

            if (date > DateTime.UtcNow)
                throw new ArgumentOutOfRangeException(nameof(date), "La fecha no puede ser futura.");

            if (!Enum.IsDefined(currency))
                throw new ArgumentOutOfRangeException(nameof(currency), "La moneda no es válida.");

            if (accountId == Guid.Empty)
                throw new ArgumentException("La cuenta es obligatoria.", nameof(accountId));

            Id = Guid.NewGuid();
            Type = type;
            Description = description;
            Amount = amount;
            Date = date;
            Currency = currency;
            AccountId = accountId;
            Cancelled = false;
        }

        public void Cancel() => Cancelled = true;
    }
}