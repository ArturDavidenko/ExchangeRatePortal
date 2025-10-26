using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Models.DTOs;
using ExchangeRatesAPI.Services;
using ExchangeRatesAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRatesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IFxRateService _fxRateService;

        public ExchangeRatesController(IFxRateService service)
        {
            _fxRateService = service;
        }

        [HttpGet("current/{region}")]
        public async Task<IActionResult> GetCurrentRates(RegionType region)
        {
            try
            {
                var rates = await _fxRateService.GetCurrentRatesFromDB(region);
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("history/{region}/{currency}")]
        public async Task<IActionResult> GetCurrencyHistory(RegionType region, string currency, [FromQuery] int days = 90)
        {
            try
            {
                var history = await _fxRateService.GetCurrencyHistory(currency, region, days);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] CalculationRequest request)
        {
            try
            {
                var result = await _fxRateService.CalculateExchange(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("trigger-update")]
        public async Task<IActionResult> TriggerManualUpdate()
        {
            try
            {
                await _fxRateService.UpdateCurrentRatesAsync();
                return Ok("Rates updated manually");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
