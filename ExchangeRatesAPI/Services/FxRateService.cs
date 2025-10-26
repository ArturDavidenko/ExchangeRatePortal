using ExchangeRatesAPI.Helper;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Models.DTOs;
using ExchangeRatesAPI.Repositories.Interfaces;
using ExchangeRatesAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace ExchangeRatesAPI.Services
{
    public class FxRateService : IFxRateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseFxRatesUrl;
        private readonly FxRateXmlParser _fxRateXmlParser;
        private readonly FxRateMapper _fxRateMapper;
        private readonly IFxRateRepository _fxRateRepository;
        private readonly FxRateCalculator _fxRateCalculator;

        public FxRateService(
            IHttpClientFactory httpClientFactory, 
            IOptions<ApiSettings> urlOptions,
            FxRateMapper fxRateMapper, 
            FxRateXmlParser fxRateXmlParser, 
            IFxRateRepository fxRateRepository,
            FxRateCalculator fxRateCalculator) 
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _baseFxRatesUrl = urlOptions.Value.BaseFxRatesUrl;
            _fxRateXmlParser = fxRateXmlParser;
            _fxRateMapper = fxRateMapper;
            _fxRateRepository = fxRateRepository;
            _fxRateCalculator = fxRateCalculator;
        }

        public async Task<FxRate> GetCurrentRatesFromDB(RegionType region)
        {
            return await _fxRateRepository.GetLatestRates(region);
        }

        public async Task<List<FxRate>> GetFxRatesListFromAPI(RegionType region)
        {
            var response = await _httpClient.GetAsync($"{_baseFxRatesUrl}/getCurrentFxRates?tp={region}");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();

            var xDoc = _fxRateXmlParser.CleanAndParseXml(xmlContent);

            var fxRatesList = _fxRateMapper.MapXmlToFxRate(xDoc);

            return fxRatesList;
        }

        public async Task<List<FxRate>> GetHistoricalFxRates(RegionType region, DateTime date)
        {
            var dateString = date.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetAsync($"{_baseFxRatesUrl}/getFxRates?tp={region}&dt={dateString}");
            response.EnsureSuccessStatusCode();
            var xmlContent = await response.Content.ReadAsStringAsync();
            var xDoc = _fxRateXmlParser.CleanAndParseXml(xmlContent);
            return _fxRateMapper.MapXmlToFxRate(xDoc);
        }

        public async Task SeedHistoricalDataAsync()
        {
            var hasData = await _fxRateRepository.AnyDataExist();

            if (hasData)
                return;

            for (int i = 0; i < 90; i++)
            {
                var date = DateTime.UtcNow.AddDays(-i);

                var euRates = await GetHistoricalFxRates(RegionType.EU, date);
                if (euRates != null && euRates.Any())
                    await _fxRateRepository.SaveRates(euRates);

                var ltRates = await GetHistoricalFxRates(RegionType.LT, date);
                if (ltRates != null && ltRates.Any())
                    await _fxRateRepository.SaveRates(ltRates);

                await Task.Delay(100);
            }
        }

        public async Task UpdateCurrentRatesAsync()
        {
            var euRates = await GetFxRatesListFromAPI(RegionType.EU);
            if (euRates != null && euRates.Any())
                await _fxRateRepository.SaveRates(euRates);

            var ltRates = await GetFxRatesListFromAPI(RegionType.LT);
            if (ltRates != null && ltRates.Any())
                await _fxRateRepository.SaveRates(ltRates);
        }

        public async Task<List<CurrencyHistoryPoint>> GetCurrencyHistory(string currency, RegionType region, int days = 90)
        {
            return await _fxRateRepository.GetCurrencyHistory(currency, region, days);
        }

        public async Task<CalculationResult> CalculateExchange(CalculationRequest request)
        {
            var latestRates = await _fxRateRepository.GetLatestRates(request.Region);

            if (latestRates == null)
                throw new Exception($"No rates found for region {request.Region}");

            return _fxRateCalculator.Calculate(request, latestRates);
        }
    }
}
