using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Domain.Exceptions;

namespace ScrumBoard.Application.Columns;
public class ColumnService(IColumnRepository columnRepository, IProjectRepository projectRepository)
{
    public async Task<List<ColumnDto>> GetByProjectIdAsync(Guid projectId)
    {
        var columns = await columnRepository.GetProjectIdAsync(projectId);
        return columns.Select(ToDto).ToList();
    }

    public async Task<ColumnDto> CreateAsync(Guid projectId, CreateColumnRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ColumnValidationException("El nombre de la columna es obligatorio.");

        var project = await projectRepository.GetByIdAsync(projectId);
        if (project is null)
            throw new ColumnValidationException("El proyecto especificado no existe.");

        var existing = (await columnRepository.GetProjectIdAsync(projectId)).ToList();
        var nextOrder = existing.Count != 0 ? existing.Max(c => c.Order) + 1 : 0;

        var column = new Column
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Order = nextOrder,
            ProjectId = projectId
        };

        await columnRepository.AddAsync(column);
        return ToDto(column);
    }

    public async Task<ColumnDto?> UpdateAsync(Guid projectId, Guid id, UpdateColumnRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ColumnValidationException("El nombre de la columna es obligatorio.");

        var column = await columnRepository.GetByIdAsync(id);
        if (column is null || column.ProjectId != projectId) return null;

        column.Name = request.Name.Trim();
        await columnRepository.UpdateAsync(column);
        return ToDto(column);
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid id)
    {
        var column = await columnRepository.GetByIdAsync(id);
        if (column is null || column.ProjectId != projectId) return false;

        var taskCount = await columnRepository.CountTasksByColumnIdAsync(id);
        if (taskCount > 0)
            throw new ColumnValidationException(
                $"No se puede eliminar la columna porque contiene {taskCount} tarea(s). Mueve o elimina las tareas primero.");

        await columnRepository.DeleteAsync(id);
        return true;
    }

    public async Task<List<ColumnDto>> ReorderAsync(Guid projectId, ReorderColumnsRequest request)
    {
        var existing = (await columnRepository.GetProjectIdAsync(projectId)).ToList();

        var requestedIds = request.OrderedColumnIds.ToHashSet();
        var actualIds = existing.Select(c => c.Id).ToHashSet();

        if (requestedIds.Count != actualIds.Count || !requestedIds.SetEquals(actualIds))
            throw new ColumnValidationException("La lista de columnas a reordenar no coincide con las columnas del proyecto.");

        for (var index = 0; index < request.OrderedColumnIds.Count; index++)
        {
            var column = existing.First(c => c.Id == request.OrderedColumnIds[index]);
            column.Order = index;
            await columnRepository.UpdateAsync(column);
        }

        return existing.OrderBy(c => c.Order).Select(ToDto).ToList();
    }

    private static ColumnDto ToDto(Column column) => new()
    {
        Id = column.Id,
        Name = column.Name,
        Order = column.Order,
        ProjectId = column.ProjectId
    };
}