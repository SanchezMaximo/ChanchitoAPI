namespace Domain.Entities
{
    public class Customer : User
    {
        public List<Account> Accounts { get; private set; } = new List<Account>();

        private Customer()
        {
        }

        public Customer(string email, string passwordHash)
            : base(email, passwordHash)
        {
        }
    }
}