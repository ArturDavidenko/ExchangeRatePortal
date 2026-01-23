using ExchangeRatesAPI.Helper;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Models.DTOs;
using ExchangeRatesAPI.Repositories.Interfaces;
using ExchangeRatesAPI.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Xml.Linq;
using TestesExchangeRatesAPI.Helpers.XmlSamples;

namespace TestesExchangeRatesAPI
{
    public class FxRateServiceTestes
    {
        private readonly Mock<IFxRateRepository> _mockRepository;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly IOptions<ApiSettings> _options;

        private readonly FxRateService _service;

        public FxRateServiceTestes()
        {
            _mockRepository = new Mock<IFxRateRepository>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var parser = new FxRateXmlParser();
            var mapper = new FxRateMapper();
            var calculator = new FxRateCalculator();

            _options = Options.Create(new ApiSettings
            {
                BaseFxRatesUrl = "http://test.com"
            });

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(ValidFxRatesXml.ValidEuRates())
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            _mockHttpClientFactory
                .Setup(x => x.CreateClient("ApiClient"))
                .Returns(httpClient);

            _service = new FxRateService(
                _mockHttpClientFactory.Object,
                _options,
                mapper,
                parser,
                _mockRepository.Object,
                calculator
            );
        }

        [Fact]
        public async Task GetCurrentRatesFromDB_WithValidRegion_ReturnsFxRate()
        {
            // Arrange
            var region = RegionType.EU;
            var expectedRate = new FxRate
            {
                Id = "1",
                Date = DateTime.UtcNow.Date,
                RegionType = region,
                Rates = new List<CurrencyRate>
                {
                    new CurrencyRate { Currency = "USD", Rate = 1.2m },
                    new CurrencyRate { Currency = "GBP", Rate = 0.9m },
                    new CurrencyRate { Currency = "JPY", Rate = 130.5m },
                    new CurrencyRate { Currency = "RUB", Rate = 95.5m }
                },
                BaseCurrency = "EUR"
            };


            _mockRepository
           .Setup(repo => repo.GetLatestRates(region))
           .ReturnsAsync(expectedRate);

            // Act 
            var result = await _service.GetCurrentRatesFromDB(region);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(region, result.RegionType);
            Assert.Equal("EUR", result.BaseCurrency);

            Assert.Equal(4, result.Rates.Count);

            var usdRate = result.Rates.FirstOrDefault(r => r.Currency == "USD");
            Assert.NotNull(usdRate);
            Assert.Equal(1.2m, usdRate.Rate);

            var gbpRate = result.Rates.FirstOrDefault(r => r.Currency == "GBP");
            Assert.NotNull(gbpRate);
            Assert.Equal(0.9m, gbpRate.Rate);

            _mockRepository.Verify(
                repo => repo.GetLatestRates(region),
                Times.Once);
        }


        [Fact]
        public async Task GetFxRatesListFromAPI_WithValidRegion_ReturnsListOfFxRates()
        {
            // Arrange
            var region = RegionType.EU;

            // Act
            var result = await _service.GetFxRatesListFromAPI(region);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            var firstRate = result.First();
            Assert.Equal(RegionType.EU, firstRate.RegionType);
            Assert.Equal("EUR", firstRate.BaseCurrency);

            var usdRate = firstRate.Rates.First(r => r.Currency == "USD");
            Assert.Equal(1.09m, usdRate.Rate);
        }


        [Fact]
        public async Task GetHistoricalFxRates_ReturnsFxRates_WhenResponseIsValid()
        {
            // Arrange
            var region = RegionType.EU;
            var date = new DateTime(2024, 01, 01);

            // Act
            var result = await _service.GetHistoricalFxRates(region, date);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            Assert.All(result, fxRate =>
            {
                Assert.NotEmpty(fxRate.Rates);

                Assert.All(fxRate.Rates, rate =>
                {
                    Assert.False(string.IsNullOrWhiteSpace(rate.Currency));
                    Assert.True(rate.Rate > 0);
                });
            });
        }


        [Fact]
        public async Task SeedHistoricalDataAsync_WhenNoDataExist_ShouldFetchAndSaveRates()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.AnyDataExist())
                .ReturnsAsync(false);

            _mockRepository
                .Setup(r => r.SaveRates(It.IsAny<List<FxRate>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.SeedHistoricalDataAsync();

            // Assert
            _mockRepository.Verify(
                r => r.SaveRates(It.Is<List<FxRate>>(rates =>
                    rates != null &&
                    rates.Any()
                )),
                Times.Once);
        }


        [Fact]
        public async Task UpdateCurrentRatesAsync_WhenApiReturnsRates_ShouldSaveRatesForBothRegions()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.SaveRates(It.IsAny<List<FxRate>>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateCurrentRatesAsync();

            // Assert
            _mockRepository.Verify(
                r => r.SaveRates(It.IsAny<List<FxRate>>()),
                Times.Exactly(2)); // EU + LT
        }

        [Fact]
        public async Task CalculateExchange_WhenRatesExist_ShouldReturnCorrectResult()
        {
            // Arrange
            var calculator = new FxRateCalculator();

            var service = new FxRateService(
                _mockHttpClientFactory.Object,
                _options,
                new FxRateMapper(),
                new FxRateXmlParser(),
                _mockRepository.Object,
                calculator
            );

            var request = new CalculationRequest
            {
                Region = RegionType.EU,
                FromCurrency = "EUR",
                ToCurrency = "USD",
                Amount = 100
            };

            var rates = new FxRate
            {
 
                RegionType = RegionType.EU,
                Date = DateTime.UtcNow.Date,
                Rates = new List<CurrencyRate>
                {
                    new CurrencyRate
                    {
                        Currency = "USD",
                        Rate = 1.1m
                    }
                }
                
            };

            _mockRepository
                .Setup(r => r.GetLatestRates(RegionType.EU))
                .ReturnsAsync(rates);
            // Act
            var result = await service.CalculateExchange(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(100, result.Amount); 
            Assert.Equal(110, result.CalculatedAmount); 
            Assert.Equal(1.1m, result.ExchangeRate); 
            Assert.Equal("EUR", result.FromCurrency);
            Assert.Equal("USD", result.ToCurrency);
            Assert.Equal(RegionType.EU, result.Region);
            Assert.Equal(rates.Date, result.RateDate);
        }



    }
}