using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Models.DTOs;
using ExchangeRatesAPI.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ExchangeRatesAPI.Repositories
{
    public class FxRateRepository : IFxRateRepository
    {
        private readonly MongoDbContext _context;

        public FxRateRepository(IOptions<MongoSettings> options)
        {
            _context = new MongoDbContext(options.Value.MongoUrl, options.Value.MongoDbName);
        }

        public async Task SaveRates(List<FxRate> rates)
        {
            foreach (var rate in rates)
            {
                var filter = Builders<FxRate>.Filter.Where(x =>
                    x.Date == rate.Date &&
                    x.RegionType == rate.RegionType);

                var update = Builders<FxRate>.Update
                    .Set(x => x.Rates, rate.Rates)
                    .Set(x => x.BaseCurrency, rate.BaseCurrency);

                await _context.FxRate.UpdateOneAsync(
                    filter,
                    update,
                    new UpdateOptions { IsUpsert = true });
            }
        }

        public async Task<FxRate> GetLatestRates(RegionType regionType)
        {
            return await _context.FxRate
            .Find(x => x.RegionType == regionType)
            .SortByDescending(x => x.Date)
            .FirstOrDefaultAsync();
        }

        public async Task<List<CurrencyHistoryPoint>> GetCurrencyHistory(string currency, RegionType region, int days = 90)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);

            var ratesFromDb = await _context.FxRate
                .Find(x => x.RegionType == region && x.Date >= startDate)
                .ToListAsync();

            var historyPoints = ratesFromDb
                .SelectMany(fxRate => fxRate.Rates
                    .Where(r => r.Currency == currency)
                    .Select(r => new CurrencyHistoryPoint
                    {
                        Date = fxRate.Date,
                        Rate = r.Rate,
                        Currency = currency,
                        RegionType = region
                    }))
                .ToList();

            return historyPoints;
        }

        public async Task<bool> AnyDataExist()
        {
            return await _context.FxRate.Find(_ => true).AnyAsync();
        }

        public async Task<bool> ExistsForDate(DateTime date, RegionType regionType)
        {
            return await _context.FxRate
                .Find(x => x.Date == date && x.RegionType == regionType)
                .AnyAsync();
        }

        public async Task<FxRate> GetRatesForDate(DateTime date, RegionType regionType)
        {
            return await _context.FxRate
                .Find(x => x.Date == date && x.RegionType == regionType)
                .FirstOrDefaultAsync();
        }
    }
}
