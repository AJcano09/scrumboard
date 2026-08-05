using ScrumBoard.Application.Common;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Domain.Enums;
using ScrumBoard.Domain.Exceptions;

namespace ScrumBoard.Application.Projects;

public class ProjectService(IProjectRepository projectRepository, IRealtimeNotifier realtimeNotifier)
{
 
    public async Task<PagedResult<ProjectDto>> GetPagedAsync(string? search, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 100 ? 10 : pageSize;

        var (items, totalCount) = await projectRepository.GetPagedAsync(
            search?.Trim().ToLower() ?? string.Empty, pageNumber, pageSize);

        return new PagedResult<ProjectDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
    
     public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await projectRepository.GetByIdAsync(id);
        return project is null ? null : ToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request)
    {
        var project = new Project(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate,
            request.Status
        );

        await projectRepository.AddAsync(project);
        await realtimeNotifier.NotifyBoardChangedAsync(project.Id, BoardHubEvents.ProjectCreated, ToDto(project));
        return ToDto(project);
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await projectRepository.GetByIdAsync(id);
        if (project is null) return null;
        
        project.Update(
            request.Name,
            request.Description ,
            request.StartDate,
            request.EndDate,
            request.Status
        );
        await projectRepository.UpdateAsync(project);
        await realtimeNotifier.NotifyBoardChangedAsync(project.Id, BoardHubEvents.ProjectUpdated, ToDto(project));
        return ToDto(project);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await projectRepository.GetByIdAsync(id);
        if (project is null) return false;

        await projectRepository.DeleteAsync(id);
        await realtimeNotifier.NotifyBoardChangedAsync(id, BoardHubEvents.ProjectDeleted, new { Id = id });
         return true;
    }

    private static ProjectDto ToDto(Project project) => new()
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        StartDate = project.StartDate,
        EndDate = project.EndDate,
        Status = project.Status.Name
    };
}