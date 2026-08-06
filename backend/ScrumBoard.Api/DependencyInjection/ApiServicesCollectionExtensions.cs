using System;
using System.Linq;
using ScrumBoard.Api.Realtime;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Api.DependencyInjection;

public static class ApiServicesCollectionExtensions
{
    public static IServiceCollection AddSignalRServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IRealtimeNotifier, SignalRBoardNotifier>();
        services.AddSignalR();
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                var allowedOrigins = configuration["Cors:AllowedOrigins"]
                    ?.Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim())
                    .ToArray();

                builder.WithOrigins(allowedOrigins ?? Array.Empty<string>())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}