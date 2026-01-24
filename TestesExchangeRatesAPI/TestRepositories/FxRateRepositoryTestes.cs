using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestesExchangeRatesAPI.Common;

namespace TestesExchangeRatesAPI.TestRepositories
{
    [Collection("MongoDb collection")]
    public class FxRateRepositoryTestes
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<FxRate> _collection;
        private readonly FxRateRepository _repository;

        public FxRateRepositoryTestes(MongoDbFixture fixture)
        {
            _database = fixture.Database;

            _collection = _database.GetCollection<FxRate>("fxrates");

            _collection.DeleteMany(FilterDefinition<FxRate>.Empty);

            var settings = new MongoSettings
            {
                MongoUrl = fixture.ConnectionString,
                MongoDbName = _database.DatabaseNamespace.DatabaseName
            };

            var options = Options.Create(settings);

            _repository = new FxRateRepository(options);
        }

        [Fact]
        public async Task SaveRates_Should_Insert_New_FxRate()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;

            var rates = new List<FxRate>
            {
                new FxRate
                {
                    Date = date,
                    RegionType = RegionType.EU,
                    BaseCurrency = "EUR",
                    Rates = new List<CurrencyRate>
                    {
                        new CurrencyRate { Currency = "USD", Rate = 1.1m },
                        new CurrencyRate { Currency = "GBP", Rate = 0.85m }
                    }
                }
            };

            // Act
            await _repository.SaveRates(rates);

            // Assert
            var saved = await _collection
                .Find(x => x.Date == date && x.RegionType == RegionType.EU)
                .FirstOrDefaultAsync();

            Assert.NotNull(saved);
            Assert.Equal("EUR", saved.BaseCurrency);
            Assert.Equal(2, saved.Rates.Count);
            Assert.Contains(saved.Rates, r => r.Currency == "USD" && r.Rate == 1.1m);
        }


        [Fact]
        public async Task SaveRates_Should_Update_Existing_FxRate()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;

            var existing = new FxRate
            {
                Date = date,
                RegionType = RegionType.LT,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate>
                {
                    new CurrencyRate { Currency = "USD", Rate = 1.0m }
                }
            };

            await _collection.InsertOneAsync(existing);

            var updated = new FxRate
            {
                Date = date,
                RegionType = RegionType.LT,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate>
                {
                    new CurrencyRate { Currency = "USD", Rate = 1.05m },
                    new CurrencyRate { Currency = "JPY", Rate = 160.2m }
                }
            };

            // Act
            await _repository.SaveRates(new List<FxRate> { updated });

            // Assert
            var result = await _collection
                .Find(x => x.Date == date && x.RegionType == RegionType.LT)
                .FirstAsync();

            Assert.Equal(2, result.Rates.Count);
            Assert.Contains(result.Rates, r => r.Currency == "JPY");
            Assert.Equal(1.05m, result.Rates.First(r => r.Currency == "USD").Rate);
        }


        [Fact]
        public async Task SaveRates_Should_Not_Create_Duplicates_For_Same_Date_And_Region()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;

            var rates = new List<FxRate>
            {
                new FxRate
                {
                    Date = date,
                    RegionType = RegionType.EU,
                    Rates = new List<CurrencyRate>
                    {
                        new CurrencyRate { Currency = "USD", Rate = 1.1m }
                    }
                },
                new FxRate
                {
                    Date = date,
                    RegionType = RegionType.EU,
                    Rates = new List<CurrencyRate>
                    {
                        new CurrencyRate { Currency = "USD", Rate = 1.2m }
                    }
                }
            };

            // Act
            await _repository.SaveRates(rates);

            // Assert
            var count = await _collection.CountDocumentsAsync(
                x => x.Date == date && x.RegionType == RegionType.EU);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task SaveRates_With_Empty_List_Should_Do_Nothing()
        {
            // Act
            await _repository.SaveRates(new List<FxRate>());

            // Assert
            var count = await _collection.CountDocumentsAsync(FilterDefinition<FxRate>.Empty);
            Assert.Equal(0, count);
        }


        [Fact]
        public async Task GetLatestRates_Should_Return_Latest_Record_For_Region()
        {
            // Arrange
            var region = RegionType.EU;

            var oldRate = new FxRate
            {
                Date = DateTime.UtcNow.AddDays(-2),
                RegionType = region,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.1m } }
            };

            var latestRate = new FxRate
            {
                Date = DateTime.UtcNow,
                RegionType = region,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.2m } }
            };

            await _collection.InsertManyAsync(new[] { oldRate, latestRate });

            // Act
            var result = await _repository.GetLatestRates(region);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(latestRate.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
             result.Date.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            Assert.Single(result.Rates);
            Assert.Equal(1.2m, result.Rates[0].Rate);
        }


        [Fact]
        public async Task GetLatestRates_Should_Return_Null_If_No_Data_For_Region()
        {
            // Arrange
            var region = RegionType.LT; // В коллекции ничего нет для LT

            // Act
            var result = await _repository.GetLatestRates(region);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetLatestRates_Should_Ignore_Other_Regions()
        {
            // Arrange
            var region = RegionType.EU;

            var otherRegion = new FxRate
            {
                Date = DateTime.UtcNow.AddDays(1),
                RegionType = RegionType.LT,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.5m } }
            };

            var correctRegion = new FxRate
            {
                Date = DateTime.UtcNow,
                RegionType = region,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.3m } }
            };

            await _collection.InsertManyAsync(new[] { otherRegion, correctRegion });

            // Act
            var result = await _repository.GetLatestRates(region);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(region, result.RegionType);
            Assert.Equal(1.3m, result.Rates[0].Rate);
        }


        [Fact]
        public async Task GetCurrencyHistory_Should_Return_Correct_History_For_Currency_And_Region()
        {
            // Arrange
            var region = RegionType.EU;
            var currency = "USD";
            var today = DateTime.UtcNow.Date;

            var oldRate = new FxRate
            {
                Date = today.AddDays(-91),
                RegionType = region,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = currency, Rate = 1.0m } }
            };

            var recentRate = new FxRate
            {
                Date = today.AddDays(-10),
                RegionType = region,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = currency, Rate = 1.2m } }
            };

            await _collection.InsertManyAsync(new[] { oldRate, recentRate });

            // Act
            var history = await _repository.GetCurrencyHistory(currency, region, days: 90);

            // Assert
            Assert.Single(history); // только recentRate
            Assert.Equal(recentRate.Date, history[0].Date);
            Assert.Equal(1.2m, history[0].Rate);
            Assert.Equal(currency, history[0].Currency);
            Assert.Equal(region, history[0].RegionType);
        }


        [Fact]
        public async Task GetCurrencyHistory_Should_Ignore_Other_Currencies()
        {
            // Arrange
            var region = RegionType.EU;
            var currency = "USD";

            var fxRate = new FxRate
            {
                Date = DateTime.UtcNow,
                RegionType = region,
                Rates = new List<CurrencyRate>
                {
                    new CurrencyRate { Currency = "EUR", Rate = 0.9m }, 
                    new CurrencyRate { Currency = "USD", Rate = 1.1m }
                }
            };

            await _collection.InsertOneAsync(fxRate);

            // Act
            var history = await _repository.GetCurrencyHistory(currency, region);

            // Assert
            Assert.Single(history);
            Assert.Equal("USD", history[0].Currency);
            Assert.Equal(1.1m, history[0].Rate);
        }


        [Fact]
        public async Task GetCurrencyHistory_Should_Ignore_Other_Regions()
        {
            // Arrange
            var currency = "USD";

            var fxRate = new FxRate
            {
                Date = DateTime.UtcNow,
                RegionType = RegionType.LT,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = currency, Rate = 1.5m } }
            };

            var correctRate = new FxRate
            {
                Date = DateTime.UtcNow,
                RegionType = RegionType.EU,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = currency, Rate = 1.2m } }
            };

            await _collection.InsertManyAsync(new[] { fxRate, correctRate });

            // Act
            var history = await _repository.GetCurrencyHistory(currency, RegionType.EU);

            // Assert
            Assert.Single(history);
            Assert.Equal(1.2m, history[0].Rate);
            Assert.Equal(RegionType.EU, history[0].RegionType);
        }


        [Fact]
        public async Task GetCurrencyHistory_Should_Return_Empty_List_If_No_Data()
        {
            // Act
            var history = await _repository.GetCurrencyHistory("USD", RegionType.EU);

            // Assert
            Assert.Empty(history);
        }

        [Fact]
        public async Task AnyDataExist_Should_Return_False_If_Empty()
        {
            // Act
            var result = await _repository.AnyDataExist();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AnyDataExist_Should_Return_True_If_Data_Exists()
        {
            // Arrange
            await _collection.InsertOneAsync(new FxRate
            {
                Date = DateTime.UtcNow,
                RegionType = RegionType.EU,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.1m } }
            });

            // Act
            var result = await _repository.AnyDataExist();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsForDate_Should_Return_True_If_Record_Exists()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var region = RegionType.EU;

            await _collection.InsertOneAsync(new FxRate
            {
                Date = date,
                RegionType = region,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.1m } }
            });

            // Act
            var exists = await _repository.ExistsForDate(date, region);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task ExistsForDate_Should_Return_False_If_No_Record()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var region = RegionType.EU;

            // Act
            var exists = await _repository.ExistsForDate(date, region);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ExistsForDate_Should_Return_False_If_Wrong_Region()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var region = RegionType.EU;

            await _collection.InsertOneAsync(new FxRate
            {
                Date = date,
                RegionType = RegionType.LT, // другой регион
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.1m } }
            });

            // Act
            var exists = await _repository.ExistsForDate(date, region);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task ExistsForDate_Should_Return_False_If_Wrong_Date()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var region = RegionType.EU;

            await _collection.InsertOneAsync(new FxRate
            {
                Date = date.AddDays(-1), // другая дата
                RegionType = region,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.1m } }
            });

            // Act
            var exists = await _repository.ExistsForDate(date, region);

            // Assert
            Assert.False(exists);
        }

        [Fact]
        public async Task GetRatesForDate_Should_Return_Record_If_Exists()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var region = RegionType.EU;

            var fxRate = new FxRate
            {
                Date = date,
                RegionType = region,
                BaseCurrency = "EUR",
                Rates = new List<CurrencyRate>
                {
                    new CurrencyRate { Currency = "USD", Rate = 1.2m }
                }
            };

            await _collection.InsertOneAsync(fxRate);

            // Act
            var result = await _repository.GetRatesForDate(date, region);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(date, result.Date);
            Assert.Equal(region, result.RegionType);
            Assert.Single(result.Rates);
            Assert.Equal(1.2m, result.Rates[0].Rate);
        }

        [Fact]
        public async Task GetRatesForDate_Should_Return_Null_If_No_Record()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;
            var region = RegionType.EU;

            // Act
            var result = await _repository.GetRatesForDate(date, region);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRatesForDate_Should_Ignore_Other_Regions()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;

            var fxRate = new FxRate
            {
                Date = date,
                RegionType = RegionType.LT,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.5m } }
            };

            await _collection.InsertOneAsync(fxRate);

            // Act
            var result = await _repository.GetRatesForDate(date, RegionType.EU);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRatesForDate_Should_Ignore_Other_Dates()
        {
            // Arrange
            var region = RegionType.EU;

            var fxRate = new FxRate
            {
                Date = DateTime.UtcNow.AddDays(-1),
                RegionType = region,
                Rates = new List<CurrencyRate> { new CurrencyRate { Currency = "USD", Rate = 1.3m } }
            };

            await _collection.InsertOneAsync(fxRate);

            // Act
            var result = await _repository.GetRatesForDate(DateTime.UtcNow.Date, region);

            // Assert
            Assert.Null(result);
        }


    }
}
