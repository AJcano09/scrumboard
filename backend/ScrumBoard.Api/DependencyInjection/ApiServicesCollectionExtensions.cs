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
                builder.WithOrigins("http://localhost:4200") 
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
        
        return services;
    }
}