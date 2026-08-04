using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Tasks;
using ScrumBoard.Domain.Exceptions;

namespace ScrumBoard.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController(TaskService taskService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskRequest request)
    {
        try { return Ok(await taskService.CreateAsync(request)); }
        catch (TaskValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskRequest request)
    {
        try
        {
            var task = await taskService.UpdateAsync(id, request);
            return task is null ? NotFound() : Ok(task);
        }
        catch (TaskValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await taskService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, MoveTaskRequest request)
    {
        try
        {
            var task = await taskService.MoveAsync(id, request);
            return task is null ? NotFound() : Ok(task);
        }
        catch (TaskValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}