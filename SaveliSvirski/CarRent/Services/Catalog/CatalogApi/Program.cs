using System.Reflection;
using CatalogApi.Extensions;
using CQRS.Commands;
using Data.Contracts;
using Data.Repository;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SharedModels.Utils;
using SignalR;
using Validator;

namespace CatalogApi
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

            builder.Services.ConfigureMassTransit(configuration)
                .ConfigureCors(configuration)
                .ConfigurePostgresContext(configuration)
                .AddDistributedMemoryCache()
                .AddAndConfigureRedisCache(configuration)
                .AddMediatR(Assembly.Load("CQRS"))
                .AddScoped<IRepositoryManager, RepositoryManager>()
                .AddAutoMapper(Assembly.Load("Mapper"))
                .AddScoped<IValidator<CarToAddCommand>, CarToAddValidator>()
                .AddScoped<IValidator<CarToUpdateCommand>, CarToUpdateValidator>()
                .Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
                .ConfigureAuthorization(configuration)
                .ConfigureSwagger()
                .AddEndpointsApiExplorer()
                .ConfigureControllers()
                .AddSignalR();

            var app = builder.Build();

            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.MigrateDb();

            app.UseExceptionHandlerMiddleware();

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<CatalogHub>("/cataloghub")
                .RequireCors("SignalRPolicy");

            app.Run();
        }
    }
}