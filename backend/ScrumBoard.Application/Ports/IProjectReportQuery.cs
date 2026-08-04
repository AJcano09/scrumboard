using ScrumBoard.Application.Reports;

namespace ScrumBoard.Application.Ports;

public interface IProjectReportQuery
{
    Task<ProjectReportDto?> GetProjectReportDataAsync(Guid projectId);
}