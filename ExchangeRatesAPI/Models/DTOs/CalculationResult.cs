namespace ExchangeRatesAPI.Models.DTOs
{
    public class CalculationResult
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal CalculatedAmount { get; set; }
        public DateTime RateDate { get; set; }
        public RegionType Region { get; set; }
    }
}
