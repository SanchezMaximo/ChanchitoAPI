namespace Domain.Entities
{
    public abstract class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }

        protected User()
        {
        }

        protected User(string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                throw new ArgumentException("The email is not valid.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("The password hash is required.", nameof(passwordHash));

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
        }

        public void Suspend()
        {
            if (!IsActive)
                throw new InvalidOperationException("The user is already suspended.");

            IsActive = false;
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new InvalidOperationException("The user is already active.");

            IsActive = true;
        }
    }
}