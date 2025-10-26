using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ExchangeRatesAPI.Models.DTOs
{
    public class CurrencyHistoryPoint
    {
        [BsonElement("date")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Date { get; set; }

        [BsonElement("rate")]
        public decimal? Rate { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; }

        [BsonElement("regionType")]
        [BsonRepresentation(BsonType.String)]
        public RegionType RegionType { get; set; }
    }
}
