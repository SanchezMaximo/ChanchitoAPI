using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<ExchangeRate> GetAsync(ExchangeRateSource source, CancellationToken cancellationToken = default);
    }
}