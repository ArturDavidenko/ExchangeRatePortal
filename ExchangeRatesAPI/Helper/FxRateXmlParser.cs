using System.Xml.Linq;

namespace ExchangeRatesAPI.Helper
{
    public class FxRateXmlParser
    {
        public XDocument CleanAndParseXml(string xmlContent)
        {
            xmlContent = xmlContent.Replace(" xmlns=\"http://www.lb.lt/WebServices/FxRates\"", "");
            return XDocument.Parse(xmlContent);
        }
    }
}
