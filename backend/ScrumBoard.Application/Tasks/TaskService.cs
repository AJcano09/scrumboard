using ScrumBoard.Application.Common;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Exceptions;
using ScrumBoard.Domain.Services;

namespace ScrumBoard.Application.Tasks;
public class TaskService(ITaskRepository taskRepository, IColumnRepository columnRepository
,IRealtimeNotifier realtimeNotifier)
{
    private static readonly string[] ValidPriorities = ["Baja", "Media", "Alta"];

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request)
    {
        Validate(request.Title, request.Priority);

        var column = await columnRepository.GetByIdAsync(request.ColumnId);
        if (column is null)
            throw new TaskValidationException("La columna especificada no existe.");

        var existingTasks = (await taskRepository.GetByColumnIdAsync(request.ColumnId)).ToList();
        var lastOrder = existingTasks.Count != 0 ? (decimal?)existingTasks.Max(t => t.Order) : null;

        var task = new Domain.Entities.Task
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Priority = request.Priority,
            ResponsibleId = request.ResponsibleId,
            ColumnId = request.ColumnId,
            Order = TaskOrderCalculator.CalculateNewOrder(lastOrder, null),
            CreatedAt = DateTime.UtcNow
        };

        await taskRepository.AddAsync(task);
        var dto =await ToDtoAsync(task);
        await realtimeNotifier.NotifyBoardChangedAsync(column.ProjectId, BoardHubEvents.TaskCreated, dto);
        return  dto;
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request)
    {
        Validate(request.Title, request.Priority);

        var task = await taskRepository.GetByIdAsync(id);
        if (task is null) return null;

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim() ?? string.Empty;
        task.Priority = request.Priority;
        task.ResponsibleId = request.ResponsibleId;

        await taskRepository.UpdateAsync(task);
        var dto = await ToDtoAsync(task);

        // Obtenemos el ProjectId a través de la columna de la tarea
        var column = await columnRepository.GetByIdAsync(task.ColumnId);
        if (column is not null)
        {
            await realtimeNotifier.NotifyBoardChangedAsync(column.ProjectId, BoardHubEvents.TaskUpdated, dto);
        }

        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await taskRepository.GetByIdAsync(id);
        if (task is null) return false;

        // Recuperamos el ProjectId antes de eliminar la tarea
        var column = await columnRepository.GetByIdAsync(task.ColumnId);
        var projectId = column?.ProjectId ?? Guid.Empty;

        await taskRepository.DeleteAsync(id);

        if (projectId != Guid.Empty)
        {
            // Notificamos pasando únicamente el Id de la tarea eliminada
            await realtimeNotifier.NotifyBoardChangedAsync(projectId, BoardHubEvents.TaskDeleted, new { Id = id });
        }

        return true;
    }

    public async Task<TaskDto?> MoveAsync(Guid id, MoveTaskRequest request)
    {
        var task = await taskRepository.GetByIdAsync(id);
        if (task is null) return null;

        var targetColumn = await columnRepository.GetByIdAsync(request.TargetColumnId);
        if (targetColumn is null)
            throw new TaskValidationException("La columna destino no existe.");

        var targetTasks = (await taskRepository.GetByColumnIdAsync(request.TargetColumnId))
            .Where(t => t.Id != id)
            .OrderBy(t => t.Order)
            .ToList();

        var newIndex = Math.Clamp(request.NewIndex, 0, targetTasks.Count);

        decimal? previousOrder = newIndex > 0 ? targetTasks[newIndex - 1].Order : null;
        decimal? nextOrder = newIndex < targetTasks.Count ? targetTasks[newIndex].Order : null;

        task.Order = TaskOrderCalculator.CalculateNewOrder(previousOrder, nextOrder);
        task.ColumnId = request.TargetColumnId;

        await taskRepository.UpdateAsync(task);
        var dto = await ToDtoAsync(task);

        // La columna destino ya fue cargada, usamos su ProjectId
        await realtimeNotifier.NotifyBoardChangedAsync(targetColumn.ProjectId, BoardHubEvents.TaskMoved, dto);
        
        return dto;
    }

    private static void Validate(string title, string priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new TaskValidationException("El título de la tarea es obligatorio.");

        if (!ValidPriorities.Contains(priority))
            throw new TaskValidationException($"La prioridad debe ser una de: {string.Join(", ", ValidPriorities)}.");
    }

    private async Task<TaskDto> ToDtoAsync(Domain.Entities.Task task)
    {
        // Reconsulta con Include para traer el nombre del responsable actualizado.
        var refreshed = (await taskRepository.GetByColumnIdAsync(task.ColumnId))
            .First(t => t.Id == task.Id);

        return new TaskDto
        {
            Id = refreshed.Id,
            Title = refreshed.Title,
            Description = refreshed.Description,
            Priority = refreshed.Priority,
            ResponsibleId = refreshed.ResponsibleId,
            ResponsibleName = refreshed.Responsible?.Name ?? string.Empty,
            ColumnId = refreshed.ColumnId,
            Order = refreshed.Order,
            CreatedAt = refreshed.CreatedAt
        };
    }
}