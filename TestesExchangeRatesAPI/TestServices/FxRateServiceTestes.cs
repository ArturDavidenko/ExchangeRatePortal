using ExchangeRatesAPI.Helper;
using ExchangeRatesAPI.Models;
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

            // Options Ч без Moq
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

    }
}