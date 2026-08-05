using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.HealthChecks;

/// <summary>
/// Health check que verifica conectividad a la base de datos mediante EF Core.
/// No requiere paquetes NuGet externos: usa ScrumBoardDbContext (ya registrado en DI)
/// con CanConnectAsync sobre la misma cadena de conexión que la app usa en runtime.
/// </summary>
public sealed class DatabaseHealthCheck : IHealthCheck
{
    private readonly ScrumBoardDbContext _context;

    public DatabaseHealthCheck(ScrumBoardDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext healthCheckContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database connection OK");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"Database unreachable: {ex.Message}");
        }
    }
}
