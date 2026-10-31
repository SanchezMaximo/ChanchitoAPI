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

        public Transaction RegisterIncome(string description, decimal amount, Guid? exchangeRateId = null, Guid? categoryId = null)
        {
            var transaction = new Transaction(TransactionType.Income, description, amount, DateTime.UtcNow, Currency, Id,
                exchangeRateId: exchangeRateId, categoryId: categoryId);
            Balance += amount;
            Transactions.Add(transaction);
            return transaction;
        }

        public Transaction RegisterExpense(string description, decimal amount, Guid? exchangeRateId = null, Guid? categoryId = null, Guid? goalId = null)
        {
            var transaction = new Transaction(TransactionType.Expense, description, amount, DateTime.UtcNow, Currency, Id,
                exchangeRateId: exchangeRateId, categoryId: categoryId, goalId: goalId);

            if (amount > Balance)
                throw new InvalidOperationException("Insufficient balance for the expense.");

            Balance -= amount;
            Transactions.Add(transaction);
            return transaction;
        }

        private Transaction GetTransaction(Guid transactionId)
        {
            var transaction = Transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transaction is null)
                throw new KeyNotFoundException($"There is no transaction with id {transactionId}.");

            return transaction;
        }

        public Transaction ChangeTransactionAmount(Guid transactionId, decimal newAmount)
        {
            var transaction = GetTransaction(transactionId);

            var difference = newAmount - transaction.Amount;

            var newBalance = transaction.Type == TransactionType.Income ? Balance + difference : Balance - difference;

            if (newBalance < 0)
                throw new InvalidOperationException("The change would leave the balance negative.");

            transaction.ChangeAmount(newAmount);
            Balance = newBalance;

            return transaction;
        }

        public Transaction CancelTransaction(Guid transactionId)
        {
            var transaction = GetTransaction(transactionId);

            var newBalance = transaction.Type == TransactionType.Income ? Balance - transaction.Amount : Balance + transaction.Amount;

            if (newBalance < 0)
                throw new InvalidOperationException("Cancelling this transaction would leave the balance negative.");

            transaction.Cancel();
            Balance = newBalance;

            return transaction;
        }
    }
}