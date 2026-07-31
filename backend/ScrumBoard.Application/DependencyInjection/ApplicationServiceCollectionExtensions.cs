using Microsoft.Extensions.DependencyInjection;

namespace ScrumBoard.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // Registrar futuros casos de uso, handlers, validadores y servicios de aplicación.
        return services;
    }
}