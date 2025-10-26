namespace ExchangeRatesAPI.Models.DTOs
{
    public class CalculationRequest
    {
        public decimal Amount { get; set; }
        public string FromCurrency { get; set; } = "EUR";
        public string ToCurrency { get; set; }
        public RegionType Region { get; set; } = RegionType.EU;
    }
}
