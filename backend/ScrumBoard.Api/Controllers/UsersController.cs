using Microsoft.AspNetCore.Mvc;
using ScrumBoard.Application.Users;

namespace ScrumBoard.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(int page, int pageSize) => Ok(await userService.GetAllPagedAsync(page,pageSize));
}