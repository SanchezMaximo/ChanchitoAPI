using Domain.Entities;

namespace Application.DTOs
{
    public record ExchangeRateResponse(Guid Id, string Source, decimal Buy, decimal Sell, DateTime UpdatedAt, bool IsStale)
    {
        public static ExchangeRateResponse From(ExchangeRate rate, bool isStale = false) =>
            new(rate.Id, rate.Source.ToString(), rate.Buy, rate.Sell, DateTime.SpecifyKind(rate.UpdatedAt, DateTimeKind.Utc), isStale);
    }
}
