using ExchangeRatesAPI.Services.Interfaces;
using Quartz;

namespace ExchangeRatesAPI.Jobs
{
    public class FxRatesJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public FxRatesJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var fxRateService = scope.ServiceProvider.GetRequiredService<IFxRateService>();

            try
            {
                await fxRateService.UpdateCurrentRatesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating rates: {ex.Message}");
            }
        }
    }
}
