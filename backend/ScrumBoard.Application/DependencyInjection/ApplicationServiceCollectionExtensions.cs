using Microsoft.Extensions.DependencyInjection;
using ScrumBoard.Application.Auth;
using ScrumBoard.Application.Columns;
using ScrumBoard.Application.Projects;

namespace ScrumBoard.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Registrar futuros casos de uso, handlers, validadores y servicios de aplicación.
        
        // 1. Services 
        services.AddScoped<AuthService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<ColumnService>();
        return services;
    }
}