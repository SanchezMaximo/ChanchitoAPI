using Domain.Enums;

namespace Domain.Entities;

public class ExchangeRate
{
    public Guid Id { get; private set; }
    public ExchangeRateSource Source { get; private set; }
    public decimal Buy { get; private set; }
    public decimal Sell { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Required by EF Core
    private ExchangeRate() { }

    public ExchangeRate(ExchangeRateSource source, decimal buy, decimal sell, DateTime updatedAt)
    {
        if (!Enum.IsDefined(source))
            throw new ArgumentException("Invalid exchange rate source.", nameof(source));

        if (buy <= 0)
            throw new ArgumentException("Buy price must be greater than zero.", nameof(buy));

        if (sell <= 0)
            throw new ArgumentException("Sell price must be greater than zero.", nameof(sell));

        Id = Guid.NewGuid();
        Source = source;
        Buy = buy;
        Sell = sell;
        UpdatedAt = updatedAt;
    }
}