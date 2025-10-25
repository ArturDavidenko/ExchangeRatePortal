using ExchangeRatesAPI.Data;
using ExchangeRatesAPI.Models;
using Microsoft.Extensions.Options;

namespace ExchangeRatesAPI.Repositories
{
    public class FxRateRepository
    {
        private readonly MongoDbContext _context;

        public FxRateRepository(IOptions<MongoSettings> options)
        {
            _context = new MongoDbContext(options.Value.MongoUrl, options.Value.MongoDbName);
        }


    }
}
