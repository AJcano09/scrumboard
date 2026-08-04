using Microsoft.Extensions.DependencyInjection;
using ScrumBoard.Application.Auth;
using ScrumBoard.Application.Board;
using ScrumBoard.Application.Columns;
using ScrumBoard.Application.Projects;
using ScrumBoard.Application.Tasks;
using ScrumBoard.Application.Users;

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
        services.AddScoped<TaskService>();
        services.AddScoped<BoardService>();
        services.AddScoped<UserService>();
        return services;
    }
}