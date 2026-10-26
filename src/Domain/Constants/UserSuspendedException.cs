namespace Domain.Exceptions
{
    public class UserSuspendedException : Exception
    {
        public UserSuspendedException(string message)
            : base(message)
        {
        }
    }
}
