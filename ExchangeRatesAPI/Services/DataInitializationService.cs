
using ExchangeRatesAPI.Services.Interfaces;

namespace ExchangeRatesAPI.Services
{
    public class DataInitializationService : IHostedService
    {

        private readonly IServiceProvider _serviceProvider;

        public DataInitializationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var fxRateService = scope.ServiceProvider.GetRequiredService<IFxRateService>();

            await fxRateService.SeedHistoricalDataAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
