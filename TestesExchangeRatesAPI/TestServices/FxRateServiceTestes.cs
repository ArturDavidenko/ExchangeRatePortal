using ExchangeRatesAPI.Helper;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Repositories.Interfaces;
using ExchangeRatesAPI.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Xml.Linq;

namespace TestesExchangeRatesAPI
{
    public class FxRateServiceTestes
    {
        private readonly Mock<IFxRateRepository> _mockRepository;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IOptions<ApiSettings>> _mockOptions;
        private readonly Mock<FxRateMapper> _mockMapper;
        private readonly Mock<FxRateXmlParser> _mockParser;
        private readonly Mock<FxRateCalculator> _mockCalculator;
        private readonly FxRateService _service;

        public FxRateServiceTestes()
        {
            _mockRepository = new Mock<IFxRateRepository>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockOptions = new Mock<IOptions<ApiSettings>>();
            _mockMapper = new Mock<FxRateMapper>();
            _mockParser = new Mock<FxRateXmlParser>();
            _mockCalculator = new Mock<FxRateCalculator>();

            // HttpClientFactory
            var mockHttpClient = new Mock<HttpClient>();
            _mockHttpClientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(mockHttpClient.Object);

            // ApiSettings
            _mockOptions.Setup(x => x.Value)
                .Returns(new ApiSettings { BaseFxRatesUrl = "http://test.com" });

            _service = new FxRateService(
                _mockHttpClientFactory.Object,
                _mockOptions.Object,
                _mockMapper.Object,
                _mockParser.Object,
                _mockRepository.Object,
                _mockCalculator.Object
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
            var expectedRates = new List<FxRate>
            {
                new FxRate
                {
                    Id = "rate-1",
                    Date = DateTime.Parse("2024-01-15"),
                    RegionType = RegionType.EU,
                    BaseCurrency = "EUR",
                    Rates = new List<CurrencyRate>
                    {
                        new CurrencyRate { Currency = "USD", Rate = 1.09m },
                        new CurrencyRate { Currency = "GBP", Rate = 0.86m }
                    }
                },
                new FxRate
                {
                    Id = "rate-2",
                    Date = DateTime.Parse("2024-01-14"),
                    RegionType = RegionType.EU,
                    BaseCurrency = "EUR",
                    Rates = new List<CurrencyRate>
                    {
                        new CurrencyRate { Currency = "USD", Rate = 1.08m },
                        new CurrencyRate { Currency = "GBP", Rate = 0.85m }
                    }
                }
            };

            // HttpClient mock
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
                    Content = new StringContent("<xml>test xml content</xml>")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var mockXDocument = new XDocument();
            _mockParser
                .Setup(x => x.CleanAndParseXml(It.IsAny<string>()))
                .Returns(mockXDocument);

            _mockMapper
                .Setup(x => x.MapXmlToFxRate(mockXDocument))
                .Returns(expectedRates);

            // Act
            var result = await _service.GetFxRatesListFromAPI(region);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<FxRate>>(result);
            Assert.Equal(2, result.Count);

            var firstRate = result[0];
            Assert.Equal("rate-1", firstRate.Id);
            Assert.Equal(RegionType.EU, firstRate.RegionType);
            Assert.Equal(2, firstRate.Rates.Count);

            var usdRate = firstRate.Rates.FirstOrDefault(r => r.Currency == "USD");
            Assert.NotNull(usdRate);
            Assert.Equal(1.09m, usdRate.Rate);
        }

    }
}