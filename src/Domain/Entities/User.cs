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
                throw new ArgumentException("El email no es válido.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("El hash del password es obligatorio.", nameof(passwordHash));

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
        }

        public void Suspend()
        {
            if (!IsActive)
                throw new InvalidOperationException("El usuario ya estaba suspendido.");

            IsActive = false;
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new InvalidOperationException("El usuario ya estaba activo.");

            IsActive = true;
        }
    }
}