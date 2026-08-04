using ScrumBoard.Application.Tasks;

namespace ScrumBoard.Application.Board;

public class BoardColumnDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public List<TaskDto> Tasks { get; init; } = [];
}