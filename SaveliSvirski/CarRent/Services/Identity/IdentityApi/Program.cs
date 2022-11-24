using System.Reflection;
using BusinessLogic.Contracts;
using BusinessLogic.Dto;
using BusinessLogic.Services;
using FluentValidation;
using IdentityApi.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SharedModels.Utils;
using Validators;

namespace IdentityApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false,
                    true)
                .Build();

            LoggerConfigurator.ConfigureLogging(configuration);
            builder.Host.UseSerilog();

            builder.Services.ConfigureCors()
                .ConfigureIdentity()
                .ConfigureSqlContext(builder.Configuration)
                .ConfigureIdentityServer(builder.Configuration)
                .ConfigureSwagger()
                .AddScoped<IValidator<UserToSignUpDto>, UserToSignUpValidator>()
                .Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; })
                .AddScoped<IAccountService, AccountService>()
                .AddAutoMapper(Assembly.Load("Mapper"))
                .AddEndpointsApiExplorer()
                .AddControllers();

            var app = builder.Build();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            await app.MigrateDbAsync();

            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseExceptionHandlerMiddleware();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseIdentityServer();

            app.MapControllers();

            app.Run();
        }
    }
}