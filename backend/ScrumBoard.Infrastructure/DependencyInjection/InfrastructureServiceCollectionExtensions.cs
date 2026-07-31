using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrumBoard.Application.Ports;
using ScrumBoard.Infrastructure.Persistence;
using ScrumBoard.Infrastructure.Repositories;
using ScrumBoard.Infrastructure.Seeders;

namespace ScrumBoard.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
       this IServiceCollection services,
       IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            //1.Db context
            var connectionString = configuration.GetConnectionString("ScrumBoardConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'ScrumBoardConnection'.");
            }

            services.AddDbContext<ScrumBoardDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(3);
                    npgsqlOptions.CommandTimeout(30);
                });
            });

            //2. Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IColumnRepository, ColumnRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            //services.AddSingleton<IPasswordHasher, PasswordHasher>(); TODO: implement IpasswordHasher
            
            
            // 3. Registrar el Seeder
            services.AddTransient<DatabaseSeeder>();
            return services;
        }
    }
}
