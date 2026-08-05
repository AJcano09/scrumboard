using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Seeders
{
    public class DatabaseSeeder(ScrumBoardDbContext context, IPasswordHasher hasher)
    {
        public async System.Threading.Tasks.Task SeedAsync()
        {
            await context.Database.MigrateAsync();

            var existingUsers = await context.Users.ToListAsync();
            if (!existingUsers.Any())
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin Default",
                    Email = "admin@scrumboard.com",
                    PasswordHash = hasher.HashPassword("Login.1234")
                };
                var adminUser2 = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin 2",
                    Email = "admin2@scrumboard.com",
                    PasswordHash = hasher.HashPassword("Login.1234")
                };

                await context.Users.AddRangeAsync(adminUser, adminUser2);
                await context.SaveChangesAsync();
                existingUsers = await context.Users.ToListAsync();
            }

            var projectSeedData = new[]
            {
                new
                {
                    Name = "Sistema Scrum Principal",
                    Description = "Proyecto corporativo para organizar el backlog y el seguimiento de entregas.",
                    StartDate = DateTime.UtcNow.AddMonths(-2),
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    StatusName = "Pending",
                    Columns = new[]
                    {
                        new { Name = "Backlog", Order = 1 },
                        new { Name = "En progreso", Order = 2 },
                        new { Name = "Listo para revisar", Order = 3 },
                        new { Name = "Completado", Order = 4 }
                    },
                    Tasks = new[]
                    {
                        new { Title = "Definir criterios de aceptación", Description = "Documentar las reglas de negocio para las historias del sprint.", Priority = "Alta", ResponsibleIndex = 0, ColumnName = "Backlog", Order = 1m, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                        new { Title = "Diseñar flujo de revisión", Description = "Preparar el proceso de aprobación del equipo para las tareas en curso.", Priority = "Media", ResponsibleIndex = 1, ColumnName = "En progreso", Order = 2m, CreatedAt = DateTime.UtcNow.AddDays(-3) },
                        new { Title = "Validar tablero con usuarios", Description = "Recoger comentarios del equipo sobre la experiencia del tablero.", Priority = "Baja", ResponsibleIndex = 0, ColumnName = "Listo para revisar", Order = 3m, CreatedAt = DateTime.UtcNow.AddDays(-1) }
                    }
                },
                new
                {
                    Name = "Migración a la Nube",
                    Description = "Planificación de la migración gradual de servicios hacia infraestructura cloud.",
                    StartDate = DateTime.UtcNow.AddMonths(1),
                    EndDate = DateTime.UtcNow.AddMonths(4),
                    StatusName = "InProgress",
                    Columns = new[]
                    {
                        new { Name = "Por revisar", Order = 1 },
                        new { Name = "En ejecución", Order = 2 },
                        new { Name = "Deployado", Order = 3 }
                    },
                    Tasks = new[]
                    {
                        new { Title = "Analizar servicios actuales", Description = "Mapear dependencias y costos asociados a la migración.", Priority = "Alta", ResponsibleIndex = 1, ColumnName = "Por revisar", Order = 1m, CreatedAt = DateTime.UtcNow.AddDays(-7) },
                        new { Title = "Configurar entornos de prueba", Description = "Crear las cuentas y recursos base para validar la migración.", Priority = "Alta", ResponsibleIndex = 0, ColumnName = "En ejecución", Order = 2m, CreatedAt = DateTime.UtcNow.AddDays(-2) },
                        new { Title = "Documentar rollback", Description = "Preparar el procedimiento de reversión para el despliegue.", Priority = "Media", ResponsibleIndex = 1, ColumnName = "Deployado", Order = 3m, CreatedAt = DateTime.UtcNow.AddDays(-1) }
                    }
                },
                new
                {
                    Name = "Auditoría de Código",
                    Description = "Seguimiento de mejoras técnicas y revisión de calidad de la base de código.",
                    StartDate = DateTime.UtcNow.AddMonths(-4),
                    EndDate = DateTime.UtcNow.AddDays(-5),
                    StatusName = "Completed",
                    Columns = new[]
                    {
                        new { Name = "Pendiente", Order = 1 },
                        new { Name = "Revisado", Order = 2 },
                        new { Name = "Cerrado", Order = 3 }
                    },
                    Tasks = new[]
                    {
                        new { Title = "Revisar seguridad de endpoints", Description = "Evaluar permisos y manejo de errores en los controladores.", Priority = "Alta", ResponsibleIndex = 0, ColumnName = "Cerrado", Order = 1m, CreatedAt = DateTime.UtcNow.AddDays(-20) },
                        new { Title = "Eliminar deuda técnica", Description = "Refactorizar módulos con mayor complejidad y menor cobertura.", Priority = "Media", ResponsibleIndex = 1, ColumnName = "Revisado", Order = 2m, CreatedAt = DateTime.UtcNow.AddDays(-10) }
                    }
                },
                new
                {
                    Name = "Rediseño del Portal",
                    Description = "Proyecto de experiencia de usuario para renovar la interfaz del portal interno.",
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddMonths(2),
                    StatusName = "Pending",
                    Columns = new[]
                    {
                        new { Name = "Por priorizar", Order = 1 },
                        new { Name = "Diseño", Order = 2 },
                        new { Name = "Aprobado", Order = 3 }
                    },
                    Tasks = new[]
                    {
                        new { Title = "Definir estructura de pantallas", Description = "Mapear la navegación principal del nuevo portal.", Priority = "Alta", ResponsibleIndex = 1, ColumnName = "Por priorizar", Order = 1m, CreatedAt = DateTime.UtcNow.AddDays(-8) },
                        new { Title = "Crear componentes reutilizables", Description = "Establecer una base visual consistente para los módulos del portal.", Priority = "Media", ResponsibleIndex = 0, ColumnName = "Diseño", Order = 2m, CreatedAt = DateTime.UtcNow.AddDays(-2) }
                    }
                },
                new
                {
                    Name = "App Móvil de Campo",
                    Description = "Aplicación para capturar incidencias y visitas en tiempo real para el equipo comercial.",
                    StartDate = DateTime.UtcNow.AddDays(-20),
                    EndDate = DateTime.UtcNow.AddMonths(3),
                    StatusName = "InProgress",
                    Columns = new[]
                    {
                        new { Name = "Investigación", Order = 1 },
                        new { Name = "Desarrollo", Order = 2 },
                        new { Name = "Pruebas", Order = 3 }
                    },
                    Tasks = new[]
                    {
                        new { Title = "Validar requisitos offline", Description = "Confirmar cómo se sincronizarán los datos sin conexión.", Priority = "Alta", ResponsibleIndex = 1, ColumnName = "Investigación", Order = 1m, CreatedAt = DateTime.UtcNow.AddDays(-12) },
                        new { Title = "Implementar formulario de incidencias", Description = "Crear la experiencia para reportar problemas desde el dispositivo móvil.", Priority = "Alta", ResponsibleIndex = 0, ColumnName = "Desarrollo", Order = 2m, CreatedAt = DateTime.UtcNow.AddDays(-3) }
                    }
                }
            };

            foreach (var projectSeed in projectSeedData)
            {
                var project = await context.Projects.FirstOrDefaultAsync(p => p.Name == projectSeed.Name);
                if (project is null)
                {
                    project = new Project(
                        Guid.NewGuid(),
                        projectSeed.Name,
                        projectSeed.Description,
                        projectSeed.StartDate,
                        projectSeed.EndDate,
                        projectSeed.StatusName);

                    context.Projects.Add(project);
                    await context.SaveChangesAsync();
                }

                foreach (var columnSeed in projectSeed.Columns)
                {
                    var exists = await context.Columns.AnyAsync(c => c.ProjectId == project.Id && c.Name == columnSeed.Name);
                    if (!exists)
                    {
                        context.Columns.Add(new Column
                        {
                            Id = Guid.NewGuid(),
                            Name = columnSeed.Name,
                            Order = columnSeed.Order,
                            ProjectId = project.Id
                        });
                    }
                }

                await context.SaveChangesAsync();

                var columnsByProject = await context.Columns
                    .Where(c => c.ProjectId == project.Id)
                    .OrderBy(c => c.Order)
                    .ToListAsync();

                foreach (var taskSeed in projectSeed.Tasks)
                {
                    var targetColumn = columnsByProject.FirstOrDefault(c => c.Name == taskSeed.ColumnName);
                    if (targetColumn is null)
                    {
                        continue;
                    }

                    var responsibleUser = existingUsers[Math.Min(taskSeed.ResponsibleIndex, existingUsers.Count - 1)];
                    var alreadyExists = await context.Tasks.AnyAsync(t => t.Title == taskSeed.Title && t.ColumnId == targetColumn.Id);
                    if (alreadyExists)
                    {
                        continue;
                    }

                    context.Tasks.Add(new Domain.Entities.Task
                    {
                        Id = Guid.NewGuid(),
                        Title = taskSeed.Title,
                        Description = taskSeed.Description,
                        Priority = taskSeed.Priority,
                        ResponsibleId = responsibleUser.Id,
                        ColumnId = targetColumn.Id,
                        Order = taskSeed.Order,
                        CreatedAt = taskSeed.CreatedAt
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }
}

