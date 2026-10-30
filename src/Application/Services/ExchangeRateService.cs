using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
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
            {
                var (rate, isStale) = await ResolveAsync(source, cancellationToken);
                responses.Add(ExchangeRateResponse.From(rate, isStale));
            }

            return responses;
        }

        public async Task<Guid> GetCurrentRateIdAsync(ExchangeRateSource source, CancellationToken cancellationToken = default)
        {
            var (rate, _) = await ResolveAsync(source, cancellationToken);

            return rate.Id;
        }

        private async Task<(ExchangeRate Rate, bool IsStale)> ResolveAsync(ExchangeRateSource source, CancellationToken cancellationToken)
        {
            try
            {
                var current = await provider.GetAsync(source, cancellationToken);
                var latest = repository.GetLatest(source);

                if (latest is null || latest.UpdatedAt != current.UpdatedAt)
                {
                    repository.Add(current);
                    repository.SaveChanges();

                    return (current, false);
                }

                return (latest, false);
            }
            catch (ExchangeRateUnavailableException)
            {
                var latest = repository.GetLatest(source)
                    ?? throw new ExchangeRateUnavailableException(
                        $"The {source} exchange rate is unavailable and there is no saved rate yet.");

                return (latest, true);
            }
        }
    }
}
