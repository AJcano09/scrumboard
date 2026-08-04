namespace ScrumBoard.Application.Columns;

public class ColumnDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public Guid ProjectId { get; init; }
}