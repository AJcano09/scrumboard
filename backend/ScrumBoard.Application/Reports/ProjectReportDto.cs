namespace ScrumBoard.Application.Reports;

public record ProjectReportDto(
    Guid ProjectId, 
    string ProjectName, 
    DateTime GeneratedAt, 
    List<TaskReportItemDto> Tasks);