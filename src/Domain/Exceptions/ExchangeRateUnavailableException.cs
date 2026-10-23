namespace Domain.Exceptions
{
    public class ExchangeRateUnavailableException : Exception
    {
        public ExchangeRateUnavailableException(string message)
            : base(message)
        {
        }

        public ExchangeRateUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}