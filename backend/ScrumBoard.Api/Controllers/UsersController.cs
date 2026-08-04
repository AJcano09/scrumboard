using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Users;

namespace ScrumBoard.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPagedAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResult = await userService.GetAllPagedAsync(pageNumber, pageSize);
        return Ok(pagedResult);
    }
}