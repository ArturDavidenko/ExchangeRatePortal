using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeRatesAPI.Models
{
    public class CurrencyAmount
    {
        [BsonElement("currency")]
        public string Currency { get; set; }

        [BsonElement("amount")]
        public decimal Amount { get; set; }
    }
}
