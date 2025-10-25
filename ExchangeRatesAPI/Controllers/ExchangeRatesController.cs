using ExchangeRatesAPI.Services;
using ExchangeRatesAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRatesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExchangeRatesController : ControllerBase
    {
        private readonly IFxRateService _service;

        public ExchangeRatesController(IFxRateService service)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> TestAPIResponse()
        {
            try
            {
                var response = await _service.GetFxRatesFromApi();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

    }
}
