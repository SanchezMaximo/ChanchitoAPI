namespace Domain.Entities
{
    public class Category
    {
        public const int MaxNameLength = 50;

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        
        public Guid? CustomerId { get; private set; }

        public bool IsSystem => CustomerId is null;

        private Category()
        {
        }

        private Category(string name, Guid? customerId)
        {
            Id = Guid.NewGuid();
            Name = NormalizeName(name);
            CustomerId = customerId;
        }

        public static Category CreateSystem(string name) => new(name, null);

        public static Category CreateForCustomer(string name, Guid customerId)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("The customer is required.", nameof(customerId));

            return new Category(name, customerId);
        }

        public void Rename(string name) => Name = NormalizeName(name);

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The category name is required.", nameof(name));

            var normalized = name.Trim();

            if (normalized.Length > MaxNameLength)
                throw new ArgumentException($"The category name cannot exceed {MaxNameLength} characters.", nameof(name));

            return normalized;
        }
    }
}