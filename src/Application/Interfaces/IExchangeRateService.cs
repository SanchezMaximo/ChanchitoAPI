using Application.DTOs;
using Domain.Enums;

namespace Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<IReadOnlyList<ExchangeRateResponse>> GetCurrentAsync(CancellationToken cancellationToken = default);

        // Id of the saved rate that is in force right now for that source.
        Task<Guid> GetCurrentRateIdAsync(ExchangeRateSource source, CancellationToken cancellationToken = default);
    }
}
