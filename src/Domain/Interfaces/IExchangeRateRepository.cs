using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IExchangeRateRepository
    {
        void Add(ExchangeRate exchangeRate);

        ExchangeRate? GetLatest(ExchangeRateSource source);

        void SaveChanges();
    }
}