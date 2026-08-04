using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Application.Reports;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Reports;

public class ProjectReportQuery(ScrumBoardDbContext context) : IProjectReportQuery
{
    public async Task<ProjectReportDto?> GetProjectReportDataAsync(Guid projectId)
    {
        var project = await context.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId)
            .Select(p => new ProjectReportDto(
                p.Id,
                p.Name,
                DateTime.UtcNow,
                p.Columns.SelectMany(c => c.Tasks.Select(t => new TaskReportItemDto(
                    t.Title,
                    c.Name,
                    t.Responsible != null ? t.Responsible.Name : "Sin asignar",
                    t.Priority
                ))).ToList()
            ))
            .FirstOrDefaultAsync();

        return project;
    }
}