using System.Reflection;
using CatalogApi.Consumers;
using Data.CatalogContext;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedModels.CatalogEvents;

namespace CatalogApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services
                .AddControllers(config =>
                {
                    config.ReturnHttpNotAcceptable = true;
                    config.RespectBrowserAcceptHeader = true;
                })
                .AddXmlDataContractSerializerFormatters();

            return services;
        }

        public static IServiceCollection ConfigureMassTransit(this IServiceCollection services,
            IConfiguration configuration)
        {
            var massTransitConfig = configuration.GetSection("MassTransit");
            var url = massTransitConfig.GetValue<string>("Url");
            var host = massTransitConfig.GetValue<string>("Host");
            var catalogQueue = massTransitConfig.GetValue<string>("CatalogQueue");
            var rentQueue = massTransitConfig.GetValue<string>("RentQueue");
            var signalRQueue = massTransitConfig.GetValue<string>("SignalRQueue");
            var userName = massTransitConfig.GetValue<string>("UserName");
            var userPas = massTransitConfig.GetValue<string>("UserPas");
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CarRentedConsumer>();
                x.AddConsumer<CarRentClosedConsumer>();
                x.AddConsumer<GetInitAmountConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(url, host, h =>
                    {
                        h.Username(userName);
                        h.Password(userPas);
                    });
                    config.ReceiveEndpoint(rentQueue, ep =>
                    {
                        ep.ConfigureConsumer<CarRentedConsumer>(provider);
                        ep.ConfigureConsumer<CarRentClosedConsumer>(provider);
                    });
                    config.ReceiveEndpoint(signalRQueue, ep =>
                    {
                        ep.ConfigureConsumer<GetInitAmountConsumer>(provider);
                    });
                }));
            });
            EndpointConvention.Map<ICarDeletedEvent>(new Uri($"queue:{catalogQueue}"));
            EndpointConvention.Map<ICarAddedEvent>(new Uri($"queue:{catalogQueue}"));
            EndpointConvention.Map<IGetInitAmountEvent>(new Uri($"queue:{signalRQueue}"));

            return services;
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration.GetSection("SignalRConfig").GetValue<string>("DefaultConnection");
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                options.AddPolicy("SignalRPolicy", builder =>
                    builder.WithOrigins(url)
                        .WithMethods("GET", "POST")
                        .AllowAnyHeader()
                        .AllowCredentials());

            });

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

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/cataloghub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogApi" });
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

        public static IServiceCollection ConfigurePostgresContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<CatalogDbContext>(opts =>
                opts.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b =>
                    b.MigrationsAssembly(Assembly.Load("Data").FullName)));
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
            
            return services;
        }
    }
}