
using ExchangeRatesAPI.Helper;
using ExchangeRatesAPI.Jobs;
using ExchangeRatesAPI.Models;
using ExchangeRatesAPI.Repositories;
using ExchangeRatesAPI.Repositories.Interfaces;
using ExchangeRatesAPI.Services;
using ExchangeRatesAPI.Services.Interfaces;
using Quartz;

namespace ExchangeRatesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("DataContext"));
            builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("UrlSettings"));

            builder.Services.AddScoped<IFxRateService, FxRateService>();
            builder.Services.AddScoped<IFxRateRepository, FxRateRepository>();
            builder.Services.AddScoped<FxRateXmlParser>();
            builder.Services.AddScoped<FxRateMapper>();
            builder.Services.AddScoped<FxRateCalculator>();
            builder.Services.AddHostedService<DataInitializationService>();
            

            builder.Services.AddQuartz(q =>
            {
                var jobKey = new JobKey("FxRatesJob");

                q.AddJob<FxRatesJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("FxRatesJob-trigger")
                    .WithCronSchedule("0 0 12 * * ?"));
            });

            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


            var app = builder.Build();
 
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
