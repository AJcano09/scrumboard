using ScrumBoard.Application.Ports;
using ScrumBoard.Infrastructure.Persistence;
using ScrumBoard.Infrastructure.Seeders;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ScrumBoard.Api.DependencyInjection;

public static class WebApplicationExtensions
{
    public static async Task ConfigurePipelineAsync(this WebApplication app)
    {
        // 1. Inicialización y Seeding de Base de Datos
        await InitializeDatabaseAsync(app);

        // 2. Pipeline HTTP
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        
        // 3. Endpoints y Hubs
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("db")
        });
        app.MapControllers();
        app.MapHub<Realtime.BoardHub>("/hubs/board");

    }

    private static async Task InitializeDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ScrumBoardDbContext>();
            var passwordHasher = services.GetRequiredService<IPasswordHasher>();
            var dbSeeder = new DatabaseSeeder(context, passwordHasher);
            await dbSeeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");
            logger.LogError(ex, "Ha ocurrido un error inicializando la base de datos.");
            throw;
        }
    }
}