using Domain.Enums;

namespace Domain.Entities
{
    public class Goal
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public decimal TargetAmount { get; private set; }
        public decimal SavedAmount { get; private set; }
        public Currency Currency { get; private set; }
        public Guid CustomerId { get; private set; }
        public bool Cancelled { get; private set; }
        public bool IsCompleted => SavedAmount >= TargetAmount;
        public decimal Remaining => TargetAmount - SavedAmount;

        private Goal()
        {
            
        }


        public Goal(string name, decimal targetAmount, Currency currency, Guid customerId)
        {
            var normalizedName = NormalizeName(name);
            
            if (targetAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(targetAmount), "The target amount must be greater than zero.");    

            if (!Enum.IsDefined(currency))
                throw new ArgumentOutOfRangeException(nameof(currency), "The currency is not valid.");

            if (customerId == Guid.Empty)
                throw new ArgumentException("The customer is required.", nameof(customerId));

            Id = Guid.NewGuid();
            Name = normalizedName;
            TargetAmount = targetAmount;
            Currency = currency;
            CustomerId = customerId;
            Cancelled = false;
        }

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The goal name is required.", nameof(name));

            return name.Trim();
        }

        public void Contribute(decimal amount)
        {
            if (Cancelled)
                throw new InvalidOperationException("The goal is cancelled and cannot receive contributions.");
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "The contribution must be greater than zero.");

            if (amount > Remaining)
                throw new InvalidOperationException($"The contribution exceeds the remaining amount ({Remaining}).");

            
            SavedAmount += amount;
        }
        
        public void Cancel()
        {
            if (Cancelled)
                throw new InvalidOperationException("The goal is already cancelled.");
            if (SavedAmount > 0)
                throw new InvalidOperationException("A goal with contributions cannot be cancelled.");
                
            
            Cancelled = true;
        }
        
        public void CancelContribution(decimal amount)
        {
            if (amount <= 0 )
                throw new ArgumentOutOfRangeException(nameof(amount), "The contribution cancellation amount must be greater than zero.");
            
            if (amount > SavedAmount)
                throw new InvalidOperationException($"The contribution cancellation amount exceeds the saved amount ({SavedAmount}).");
            
            
            SavedAmount -= amount;
        }
        
        public void AdjustContribution(decimal oldAmount, decimal newAmount)
        {
            if (newAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(newAmount), "The contribution must be greater than zero.");

            var difference = newAmount - oldAmount;

            if (difference == 0)
                return;

            if (difference > Remaining)
                throw new InvalidOperationException($"The change exceeds the remaining amount ({Remaining}).");

            if (SavedAmount + difference < 0)
                throw new InvalidOperationException($"The change would leave the saved amount negative ({SavedAmount}).");

            SavedAmount += difference;
        }

    }


}