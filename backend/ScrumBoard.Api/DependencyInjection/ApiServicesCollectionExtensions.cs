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
        return services;
    }
}