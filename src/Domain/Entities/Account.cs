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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre de la cuenta es obligatorio.", nameof(name));

            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(balance), "El saldo no puede ser negativo.");

            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type), "El tipo de cuenta no es válido.");

            if (!Enum.IsDefined(currency))
                throw new ArgumentOutOfRangeException(nameof(currency), "La moneda no es válida.");

            if (customerId == Guid.Empty)
                throw new ArgumentException("El cliente es obligatorio.", nameof(customerId));

            Id = Guid.NewGuid();
            Name = name;
            Balance = balance;
            Type = type;
            Currency = currency;
            CustomerId = customerId;
        }

        public Transaction RegisterIncome(decimal amount, string description, DateTime date)
        {
            // Transaction valida monto, descripción y fecha. La moneda siempre es la de la cuenta.
            var transaction = new Transaction(TransactionType.Income, description, amount, date, Currency, Id);

            Balance += amount;
            Transactions.Add(transaction);
            return transaction;
        }

        public Transaction RegisterExpense(decimal amount, string description, DateTime date)
        {
            // Se crea primero para que un argumento inválido (400) gane sobre saldo insuficiente (409).
            var transaction = new Transaction(TransactionType.Expense, description, amount, date, Currency, Id);

            if (amount > Balance)
                throw new InvalidOperationException("No se puede registrar un egreso mayor al saldo disponible.");

            Balance -= amount;
            Transactions.Add(transaction);
            return transaction;
        }

        public void CancelTransaction(Guid transactionId)
        {
            var transaction = GetTransaction(transactionId)
                ?? throw new InvalidOperationException($"No existe el movimiento {transactionId}.");

            if (transaction.Cancelled)
                throw new InvalidOperationException("El movimiento ya estaba cancelado.");

            if (transaction.Type == TransactionType.Income && transaction.Amount > Balance)
                throw new InvalidOperationException("No se puede cancelar el ingreso: el saldo quedaría negativo.");

            transaction.Cancel();

            Balance += transaction.Type == TransactionType.Income
                ? -transaction.Amount
                : transaction.Amount;
        }

        public Transaction? GetTransaction(Guid transactionId) =>
            Transactions.FirstOrDefault(t => t.Id == transactionId);

        public IReadOnlyList<Transaction> GetHistory() => Transactions.AsReadOnly();
    }
}