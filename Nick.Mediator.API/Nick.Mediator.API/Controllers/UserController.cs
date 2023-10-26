using Microsoft.AspNetCore.Mvc;
using Nick.Mediator.API.Controllers.Base;
using Nick.Mediator.Application.Handlers.User.Queries;

namespace Nick.Mediator.API.Controllers;

/// <summary>
/// user
/// </summary>
[Route("api/user")]
public class UserController : BaseApiController
{
    [HttpPost("get-user-info")]
    public async Task<IActionResult> GetUserInfo([FromBody] GetUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await QueryAsync(new GetUserGird(request));
        return Ok(result);
    }
}