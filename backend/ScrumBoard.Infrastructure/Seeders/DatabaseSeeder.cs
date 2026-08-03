using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Domain.Enums;
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

                var project1Id = Guid.NewGuid();
                var project2Id = Guid.NewGuid();
                var project3Id = Guid.NewGuid();
                var sampleProjects = new List<Project>
                {
                    new Project
                    {
                        Id = project1Id, 
                        Name = "Sistema Scrum Principal",
                        Description = "Este es un proyecto inicial generado automáticamente.",
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        Status = ProjectStatus.Pending 
                    },
                    new Project
                    {
                        Id = project2Id,
                        Name = "Migración a la Nube",
                        Description = "Proyecto planeado para la siguiente fase.", 
                        StartDate = DateTime.UtcNow.AddMonths(1),
                        EndDate = DateTime.UtcNow.AddMonths(3),
                        Status = ProjectStatus.InProgres 
                    },
                    new Project
                    {
                        Id = project3Id,
                        Name = "Auditoría de Código",
                        Description = "Proyecto completado exitosamente.",
                        StartDate = DateTime.UtcNow.AddMonths(-2),
                        EndDate = DateTime.UtcNow.AddMonths(-1),
                        Status = ProjectStatus.Completed
                    }
                };

                await context.Projects.AddRangeAsync(sampleProjects);

                var columns = new List<Column>
                {
                    new Column { Id = Guid.NewGuid(), Name = "To Do", Order = 1, ProjectId = project1Id },
                    new Column { Id = Guid.NewGuid(), Name = "In Progress", Order = 2, ProjectId = project2Id },
                    new Column { Id = Guid.NewGuid(), Name = "Done", Order = 3, ProjectId = project3Id }
                };

                await context.Columns.AddRangeAsync(columns);

                await context.SaveChangesAsync();
            }
        }
    }
}

