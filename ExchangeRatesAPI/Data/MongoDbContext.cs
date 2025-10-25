using ExchangeRatesAPI.Models;
using MongoDB.Driver;

namespace ExchangeRatesAPI.Data
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<FxRate> FxRate => _database.GetCollection<FxRate>("fxrates");
    }
}
