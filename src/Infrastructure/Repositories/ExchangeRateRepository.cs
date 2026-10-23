using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly AppDbContext context;

        public ExchangeRateRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void Add(ExchangeRate exchangeRate) => context.ExchangeRates.Add(exchangeRate);

        public ExchangeRate? GetLatest(ExchangeRateSource source) =>
            context.ExchangeRates
                .Where(e => e.Source == source)
                .OrderByDescending(e => e.UpdatedAt)
                .FirstOrDefault();

        public void SaveChanges() => context.SaveChanges();
    }
}