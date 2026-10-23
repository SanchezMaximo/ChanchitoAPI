using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateProvider provider;
        private readonly IExchangeRateRepository repository;

        public ExchangeRateService(IExchangeRateProvider provider, IExchangeRateRepository repository)
        {
            this.provider = provider;
            this.repository = repository;
        }

        public async Task<IReadOnlyList<ExchangeRateResponse>> GetCurrentAsync(CancellationToken cancellationToken = default)
        {
            var responses = new List<ExchangeRateResponse>();

            foreach (var source in Enum.GetValues<ExchangeRateSource>())
                responses.Add(await GetBySourceAsync(source, cancellationToken));

            return responses;
        }

        private async Task<ExchangeRateResponse> GetBySourceAsync(ExchangeRateSource source, CancellationToken cancellationToken)
        {
            try
            {
                var current = await provider.GetAsync(source, cancellationToken);
                var latest = repository.GetLatest(source);

                // Only store it when the provider published a new rate.
                if (latest is null || latest.UpdatedAt != current.UpdatedAt)
                {
                    repository.Add(current);
                    repository.SaveChanges();
                }

                return ExchangeRateResponse.From(current);
            }
            catch (ExchangeRateUnavailableException)
            {
                var latest = repository.GetLatest(source)
                    ?? throw new ExchangeRateUnavailableException(
                        $"The {source} exchange rate is unavailable and there is no saved rate yet.");

                return ExchangeRateResponse.From(latest, isStale: true);
            }
        }
    }
}