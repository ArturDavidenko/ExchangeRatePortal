using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Xml.Linq;


namespace ExchangeRatesAPI.Services
{
    public class FxRateService : IFxRateService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseFxRatesUrl;

        public FxRateService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> urlOptions) 
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _baseFxRatesUrl = urlOptions.Value.BaseFxRatesUrl;
        }

        public async Task<List<FxRate>> GetFxRatesFromApi()
        {
            var response = await _httpClient.GetAsync($"{_baseFxRatesUrl}/getCurrentFxRates?tp=EU");
            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();

            xmlContent = xmlContent.Replace(" xmlns=\"http://www.lb.lt/WebServices/FxRates\"", "");

            var xDoc = XDocument.Parse(xmlContent);

            var fxRatesByDate = xDoc.Descendants("FxRate")
                .GroupBy(fxRate => new
                {
                    Date = fxRate.Element("Dt")?.Value,
                    Type = fxRate.Element("Tp")?.Value
                })
                .Where(g => !string.IsNullOrEmpty(g.Key.Date) && !string.IsNullOrEmpty(g.Key.Type));

            var fxRatesList = new List<FxRate>();

            foreach (var group in fxRatesByDate)
            {
                var currencyRates = new List<CurrencyRate>();

                foreach (var fxRate in group)
                {
                    var ccyAmts = fxRate.Elements("CcyAmt");
                    foreach (var ccyAmt in ccyAmts)
                    {
                        var currency = ccyAmt.Element("Ccy")?.Value;
                        var amount = ccyAmt.Element("Amt")?.Value;

                        if (currency != "EUR" && !string.IsNullOrEmpty(currency) && !string.IsNullOrEmpty(amount))
                        {
                            currencyRates.Add(new CurrencyRate
                            {
                                Currency = currency,
                                Rate = decimal.Parse(amount, CultureInfo.InvariantCulture)
                            });
                        }
                    }
                }

                fxRatesList.Add(new FxRate
                {
                    RegionType = Enum.Parse<RegionType>(group.Key.Type),
                    Date = DateTime.Parse(group.Key.Date),
                    BaseCurrency = "EUR",
                    Rates = currencyRates
                });
            }

            return fxRatesList;
        }
    }
}
