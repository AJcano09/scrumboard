using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Board;

namespace ScrumBoard.Api.Controllers;
[ApiController]
[Route("api/projects/{projectId:guid}/board")]
public class BoardController(BoardService boardService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid projectId) => Ok(await boardService.GetBoardAsync(projectId));
}