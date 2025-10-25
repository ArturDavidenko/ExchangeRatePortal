using ExchangeRatesAPI.Models;

namespace ExchangeRatesAPI.Services.Interfaces
{
    public interface IFxRateService
    {
        public Task<List<FxRate>> GetFxRatesFromApi();
    }
}
