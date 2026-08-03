using ScrumBoard.Domain.Enums;

namespace ScrumBoard.Application.Projects;

public class CreateProjectRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Pending;
}