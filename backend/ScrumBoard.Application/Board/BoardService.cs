using ScrumBoard.Application.Ports;
using ScrumBoard.Application.Tasks;

namespace ScrumBoard.Application.Board;

public class BoardService(IColumnRepository columnRepository, ITaskRepository taskRepository)
{
    public async Task<BoardDto> GetBoardAsync(Guid projectId)
    {
        var columns = (await columnRepository.GetProjectIdAsync(projectId)).OrderBy(c => c.Order).ToList();
        var columnDtos = new List<BoardColumnDto>();

        foreach (var column in columns)
        {
            var tasks = (await taskRepository.GetByColumnIdAsync(column.Id)).OrderBy(t => t.Order);
            columnDtos.Add(new BoardColumnDto
            {
                Id = column.Id,
                Name = column.Name,
                Order = column.Order,
                Tasks = tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    ResponsibleId = t.ResponsibleId,
                    ResponsibleName = t.Responsible?.Name ?? string.Empty,
                    ColumnId = t.ColumnId,
                    Order = t.Order,
                    CreatedAt = t.CreatedAt
                }).ToList()
            });
        }

        return new BoardDto { ProjectId = projectId, Columns = columnDtos };
    }
}