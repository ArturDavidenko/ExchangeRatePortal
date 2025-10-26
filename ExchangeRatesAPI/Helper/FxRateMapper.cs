using ExchangeRatesAPI.Models;
using System.Globalization;
using System.Xml.Linq;

namespace ExchangeRatesAPI.Helper
{
    public class FxRateMapper
    {
        public List<FxRate> MapXmlToFxRate(XDocument xDoc)
        {
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

                        if (currency != "EUR" &&
                            !string.IsNullOrEmpty(currency) &&
                            !string.IsNullOrEmpty(amount))
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
