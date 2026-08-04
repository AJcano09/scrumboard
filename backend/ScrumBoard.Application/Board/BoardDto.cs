namespace ScrumBoard.Application.Board;

public class BoardDto
{
    public Guid ProjectId { get; init; }
    public List<BoardColumnDto> Columns { get; init; } = [];
}