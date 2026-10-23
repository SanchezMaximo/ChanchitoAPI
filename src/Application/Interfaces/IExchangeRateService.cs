using Application.DTOs;

namespace Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<IReadOnlyList<ExchangeRateResponse>> GetCurrentAsync(CancellationToken cancellationToken = default);
    }
}