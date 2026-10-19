using Domain.Enums;

namespace Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal Balance { get; private set; }
        public AccountType Type { get; private set; }
        public Currency Currency { get; private set; }
        public Guid CustomerId { get; private set; }

        public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

        private Account()
        {
        }

        public Account(string name, decimal balance, AccountType type, Currency currency, Guid customerId)
        {
            var normalizedName = NormalizeName(name); 

            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(balance), "El saldo no puede ser negativo.");

            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type), "El tipo de cuenta no es válido.");

            if (!Enum.IsDefined(currency))
                throw new ArgumentOutOfRangeException(nameof(currency), "La moneda no es válida.");

            if (customerId == Guid.Empty)
                throw new ArgumentException("El cliente es obligatorio.", nameof(customerId));

            Id = Guid.NewGuid();
            Name = normalizedName;
            Balance = balance;
            Type = type;
            Currency = currency;
            CustomerId = customerId;
        }

        public void Rename(string name) => Name = NormalizeName(name);

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The account name is required.", nameof(name));

            return name.Trim();
        }
    }
}