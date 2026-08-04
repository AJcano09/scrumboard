namespace ScrumBoard.Application.Tasks;

public class TaskDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public Guid ResponsibleId { get; init; }
    public string ResponsibleName { get; init; } = string.Empty;
    public Guid ColumnId { get; init; }
    public decimal Order { get; init; }
    public DateTime CreatedAt { get; init; }
}