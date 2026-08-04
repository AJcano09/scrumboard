using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Columns;
using ScrumBoard.Domain.Exceptions;

namespace ScrumBoard.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/columns")]
public class ColumnsController(ColumnService columnService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByProject(Guid projectId)
        => Ok(await columnService.GetByProjectIdAsync(projectId));

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, CreateColumnRequest request)
    {
        try
        {
            var column = await columnService.CreateAsync(projectId, request);
            return CreatedAtAction(nameof(GetByProject), new { projectId }, column);
        }
        catch (ColumnValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid projectId, Guid id, UpdateColumnRequest request)
    {
        try
        {
            var column = await columnService.UpdateAsync(projectId, id, request);
            return column is null ? NotFound() : Ok(column);
        }
        catch (ColumnValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid id)
    {
        try
        {
            var deleted = await columnService.DeleteAsync(projectId, id);
            return deleted ? NoContent() : NotFound();
        }
        catch (ColumnValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(Guid projectId, ReorderColumnsRequest request)
    {
        try { return Ok(await columnService.ReorderAsync(projectId, request)); }
        catch (ColumnValidationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}