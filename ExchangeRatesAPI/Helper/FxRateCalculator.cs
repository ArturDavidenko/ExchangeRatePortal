using ExchangeRatesAPI.Models.DTOs;
using ExchangeRatesAPI.Models;

namespace ExchangeRatesAPI.Helper
{
    public class FxRateCalculator
    {
        public CalculationResult Calculate(CalculationRequest request, FxRate latestRates)
        {
            if (latestRates == null)
                throw new ArgumentNullException(nameof(latestRates), "Rates cannot be null");

            if (request.Amount <= 0)
                throw new ArgumentException("Amount must be greater than 0");

            var fromRate = GetRateForCurrency(latestRates, request.FromCurrency);
            var toRate = GetRateForCurrency(latestRates, request.ToCurrency);

            var calculatedAmount = CalculateAmount(request.Amount, request.FromCurrency, request.ToCurrency, fromRate, toRate);

            return new CalculationResult
            {
                Amount = request.Amount,
                FromCurrency = request.FromCurrency,
                ToCurrency = request.ToCurrency,
                ExchangeRate = toRate.Rate,
                CalculatedAmount = Math.Round(calculatedAmount, 4),
                RateDate = latestRates.Date,
                Region = request.Region
            };
        }

        private static CurrencyRate GetRateForCurrency(FxRate rates, string currency)
        {
            if (currency == "EUR")
                return new CurrencyRate { Currency = "EUR", Rate = 1.0m };

            var rate = rates.Rates.FirstOrDefault(r => r.Currency == currency);
            if (rate == null)
                throw new Exception($"Currency {currency} not found in {rates.RegionType} region");

            return rate;
        }

        private static decimal CalculateAmount(
            decimal amount,
            string fromCurrency,
            string toCurrency,
            CurrencyRate fromRate,
            CurrencyRate toRate)
        {
            var amountInEur = amount / fromRate.Rate;
            return amountInEur * toRate.Rate;
        }

    }
}
