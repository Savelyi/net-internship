using System.Reflection;
using BusinessLogic.Contracts;
using BusinessLogic.Services;
using Data.Contracts;
using Data.Repository;
using Hangfire;
using Microsoft.AspNetCore.HttpOverrides;
using RentApi.Extensions;
using Serilog;
using SharedModels.Utils;

namespace RentApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false,
                    true)
                .Build();

            LoggerConfigurator.ConfigureLogging(configuration);
            builder.Host.UseSerilog();

            builder.Services
                .ConfigureMassTransit(configuration)
                .ConfigureCors()
                .AddAutoMapper(Assembly.Load("Mapper"))
                .AddDistributedMemoryCache()
                .AddAndConfigureRedisCache(configuration)
                .ConfigurePostgresContext(configuration)
                .AddScoped<IRepositoryManager, RepositoryManager>()
                .AddScoped<IRentService, RentService>()
                .AddScoped<IRentJobsService, RentJobsService>()
                .ConfigureAuthorization(configuration)
                .ConfigureHangfire(configuration)
                .ConfigureSwagger()
                .AddEndpointsApiExplorer()
                .AddControllers();

            var app = builder.Build();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MigrateDb();
            app.UseExceptionHandlerMiddleware();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });


            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");


            app.MapControllers();

            app.UseHangfireDashboard();

            app.Run();
        }
    }
}