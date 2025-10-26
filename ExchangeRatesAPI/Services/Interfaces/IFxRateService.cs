using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Models.DTOs;

namespace ExchangeRatesAPI.Services.Interfaces
{
    public interface IFxRateService
    {
        public Task<List<FxRate>> GetFxRatesListFromAPI(RegionType region);

        public Task<FxRate> GetCurrentRatesFromDB(RegionType region);

        public Task<List<FxRate>> GetHistoricalFxRates(RegionType region, DateTime date);

        public Task SeedHistoricalDataAsync();

        public Task UpdateCurrentRatesAsync();

        public Task<List<CurrencyHistoryPoint>> GetCurrencyHistory(string currency, RegionType region, int days = 90);

        public Task<CalculationResult> CalculateExchange(CalculationRequest request);
    }
}
