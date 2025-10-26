using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Models.DTOs;

namespace ExchangeRatesAPI.Repositories.Interfaces
{
    public interface IFxRateRepository
    {
        public Task SaveRates(List<FxRate> rates);

        public Task<FxRate> GetLatestRates(RegionType regionType);

        public Task<List<CurrencyHistoryPoint>> GetCurrencyHistory(string currency, RegionType region, int days);

        public Task<bool> AnyDataExist();

        public Task<bool> ExistsForDate(DateTime date, RegionType regionType);

        public Task<FxRate> GetRatesForDate(DateTime date, RegionType regionType);



    }
}
