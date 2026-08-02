using Microsoft.Extensions.DependencyInjection;
using ScrumBoard.Application.Auth;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Registrar futuros casos de uso, handlers, validadores y servicios de aplicación.
        
        // 1. Services 
        services.AddScoped<AuthService>();
        
        return services;
    }
}