namespace ScrumBoard.Application.Tasks;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public Guid ResponsibleId { get; set; }
    public Guid ColumnId { get; set; }
}