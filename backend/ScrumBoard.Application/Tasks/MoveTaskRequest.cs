namespace ScrumBoard.Application.Tasks;

public class MoveTaskRequest
{
    public Guid TargetColumnId { get; set; }
    public int NewIndex { get; set; }
}