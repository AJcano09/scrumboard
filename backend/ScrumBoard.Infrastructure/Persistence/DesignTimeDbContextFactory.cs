
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ScrumBoard.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ScrumBoardDbContext>
{
        public ScrumBoardDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            string basePath = Directory.GetCurrentDirectory();

            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = Path.Combine(basePath, "..", "ScrumBoard.Api");
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("ScrumBoardConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Could not find 'ScrumBoardConnection' in appsettings configuration.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ScrumBoardDbContext>();

            optionsBuilder.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(ScrumBoardDbContext).Assembly.FullName));

            return new ScrumBoardDbContext(optionsBuilder.Options);
        }
    }
}
