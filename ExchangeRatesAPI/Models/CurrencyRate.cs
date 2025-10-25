using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeRatesAPI.Models
{
    public class CurrencyRate
    {
        [BsonElement("currency")]
        public string Currency { get; set; }

        [BsonElement("amount")]
        public decimal Rate { get; set; }
    }
}
