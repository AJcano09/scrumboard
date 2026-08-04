namespace ScrumBoard.Application.Reports;

public record TaskReportItemDto(
    string TaskTitle, 
    string ColumnName, 
    string ResponsibleName, 
    string Priority);