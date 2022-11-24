using System.Reflection;
using Data.HangfireContext;
using Data.RentContext;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RentApi.Consumers;
using SharedModels.Cache;
using SharedModels.RentEvents;

namespace RentApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
            IConfiguration configuration)
        {
            var massTransitConfig = configuration.GetSection("MassTransit");
            var url = massTransitConfig.GetValue<string>("Url");
            var host = massTransitConfig.GetValue<string>("Host");
            var catalogQueue = massTransitConfig.GetValue<string>("CatalogQueue");
            var rentQueue = massTransitConfig.GetValue<string>("RentQueue");
            var userName = massTransitConfig.GetValue<string>("UserName");
            var userPas = massTransitConfig.GetValue<string>("UserPas");
            if (massTransitConfig == null || url == null || host == null)
            {
                throw new ArgumentNullException(
                    "Section 'MassTransit' configuration setting are not found in appSettings.json");
            }

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CarAddedConsumer>();
                x.AddConsumer<CarDeletedConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(url, host, h =>
                    {
                        h.Username(userName);
                        h.Password(userPas);
                    });
                    config.ReceiveEndpoint(catalogQueue, ep =>
                    {
                        ep.ConfigureConsumer<CarAddedConsumer>(provider);
                        ep.ConfigureConsumer<CarDeletedConsumer>(provider);
                    });
                }));
                EndpointConvention.Map<ICarRentedEvent>(new Uri($"queue:{rentQueue}"));
                EndpointConvention.Map<ICarRentClosedEvent>(new Uri($"queue:{rentQueue}"));
            });

            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            return services;
        }

        public static IServiceCollection ConfigurePostgresContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<RentDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b =>
                {
                    b.MigrationsAssembly(Assembly.Load("Data").FullName);
                }));

            services.AddDbContext<HangfireDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("HangfireConnection")));

            return services;
        }

        public static IServiceCollection ConfigureAuthorization(this IServiceCollection services,
            IConfiguration configuration)
        {
            var authConfig = configuration.GetSection("AuthConfig");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.Authority = authConfig.GetValue<string>("Authority");
                options.Audience = authConfig.GetValue<string>("Audience");
            });

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo {Title = "RentApi"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddAndConfigureRedisCache(this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("Redis");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    $"{redisConfig.GetValue<string>("RedisHost")}:{redisConfig.GetValue<string>("RedisPort")}";
                options.InstanceName = redisConfig.GetValue<string>("InstanceName");
            });
            services.Configure<RedisOptions>(options =>
            {
                options.Host = redisConfig.GetValue<string>("RedisHost");
                options.Port = redisConfig.GetValue<int>("RedisPort");
            });
            return services;
        }

        public static IServiceCollection ConfigureHangfire(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(configuration.GetConnectionString("HangfireConnection"));
            });

            services.AddHangfireServer();

            return services;
        }
    }
}