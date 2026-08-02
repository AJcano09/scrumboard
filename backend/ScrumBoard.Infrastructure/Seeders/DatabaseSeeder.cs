using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Seeders
{
    public class DatabaseSeeder(ScrumBoardDbContext context,IPasswordHasher hasher)
    {
        public async System.Threading.Tasks.Task SeedAsync()
        {
            // Aplica las migraciones pendientes automáticamente
            await context.Database.MigrateAsync();

            // Verificar si ya existen usuarios
            if (!await context.Users.AnyAsync())
            {
                var adminUserId = Guid.NewGuid();
                var adminUser2Id = Guid.NewGuid();

                var adminUser = new User
                {
                    Id = adminUserId,
                    Name = "Admin Default",
                    Email = "admin@scrumboard.com",
                    PasswordHash = hasher.HashPassword("Login.1234")
                };
                var adminUser2 = new User
                {
                    Id = adminUser2Id,
                    Name = "Admin 2",
                    Email = "admin2@scrumboard.com",
                    PasswordHash = hasher.HashPassword("Login.1234")
                };

                await context.Users.AddAsync(adminUser);
                await context.Users.AddAsync(adminUser2);

                var projectId = Guid.NewGuid();
                var sampleProject = new Project
                {
                    Id = projectId,
                    Name = "Proyecto de Ejemplo",
                    Description = "Este es un proyecto inicial generado automáticamente.",
                    Order = 1,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                    Status = true
                };

                await context.Projects.AddAsync(sampleProject);

                var columns = new List<Column>
                {
                    new Column { Id = Guid.NewGuid(), Name = "To Do", Order = 1, ProjectId = projectId },
                    new Column { Id = Guid.NewGuid(), Name = "In Progress", Order = 2, ProjectId = projectId },
                    new Column { Id = Guid.NewGuid(), Name = "Done", Order = 3, ProjectId = projectId }
                };

                await context.Columns.AddRangeAsync(columns);

                await context.SaveChangesAsync();
            }
        }
    }
}

