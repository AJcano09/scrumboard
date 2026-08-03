using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Common;
using ScrumBoard.Application.Projects;
using ScrumBoard.Domain.Entities;

namespace ScrumBoard.Api.Controllers;

[ApiController]
[Route("api/projects")]

public class ProjectsController(ProjectService projectService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProjectDto>>> GetPaged(
        [FromQuery] string? search,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize)
    {
        var result = await projectService.GetPagedAsync(search, pageNumber, pageSize);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await projectService.GetByIdAsync(id);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest request)
    {
        try
        {
            var project = await projectService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
        }
        catch (ProjectValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectRequest request)
    {
        try
        {
            var project = await projectService.UpdateAsync(id, request);
            return project is null ? NotFound() : Ok(project);
        }
        catch (ProjectValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await projectService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}