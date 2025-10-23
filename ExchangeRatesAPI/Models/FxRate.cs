using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExchangeRatesAPI.Models
{
    public enum RegionType
    {
        EU,
        LT
    }

    public class FxRate
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("regionType")]
        [BsonRepresentation(BsonType.String)]
        public RegionType RegionType { get; set; }

        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; }

        [BsonElement("baseCurrency")]
        public string BaseCurrency { get; set; } = "EUR";

        [BsonElement("rates")]
        public List<CurrencyRate> Rates { get; set; } = new List<CurrencyRate>();
    }
}
