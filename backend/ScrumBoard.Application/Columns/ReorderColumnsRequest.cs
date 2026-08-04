namespace ScrumBoard.Application.Columns;

public class ReorderColumnsRequest
{
    public List<Guid> OrderedColumnIds { get; set; } = [];
}