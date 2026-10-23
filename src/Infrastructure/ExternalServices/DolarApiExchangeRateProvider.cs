using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Infrastructure.ExternalServices
{
    public class DolarApiExchangeRateProvider : IExchangeRateProvider
    {
        private readonly HttpClient httpClient;

        public DolarApiExchangeRateProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ExchangeRate> GetAsync(ExchangeRateSource source, CancellationToken cancellationToken = default)
        {
            var house = ToHouse(source);

            try
            {
                var rates = await httpClient.GetFromJsonAsync<List<DolarApiRate>>("v1/dolares", cancellationToken)
                    ?? throw new ExchangeRateUnavailableException("DolarAPI returned an empty response.");

                var rate = rates.FirstOrDefault(r => r.House == house)
                    ?? throw new ExchangeRateUnavailableException($"DolarAPI has no rate for '{house}'.");

                return new ExchangeRate(source, rate.Buy, rate.Sell, rate.UpdatedAt);
            }
            catch (HttpRequestException ex)
            {
                throw new ExchangeRateUnavailableException("DolarAPI could not be reached.", ex);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new ExchangeRateUnavailableException("DolarAPI took too long to respond.", ex);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException or ArgumentException)
            {
                throw new ExchangeRateUnavailableException("DolarAPI returned an invalid response.", ex);
            }
        }

        private static string ToHouse(ExchangeRateSource source) => source switch
        {
            ExchangeRateSource.Official => "oficial",
            ExchangeRateSource.Mep => "bolsa",
            _ => throw new ArgumentOutOfRangeException(nameof(source), "Unsupported exchange rate source.")
        };

        private sealed record DolarApiRate(
            [property: JsonPropertyName("casa")] string House,
            [property: JsonPropertyName("compra")] decimal Buy,
            [property: JsonPropertyName("venta")] decimal Sell,
            [property: JsonPropertyName("fechaActualizacion")] DateTime UpdatedAt);
    }
}
