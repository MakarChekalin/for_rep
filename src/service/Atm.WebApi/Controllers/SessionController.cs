using Atm.Application;
using Atm.WebApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Atm.WebApi.Controllers;

[ApiController]
[Route("api")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public SessionController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    [HttpPost("user-sessions")]
    public async Task<IActionResult> CreateUserSession([FromBody] LoginUserRequest request)
    {
        Guid? key = await _sessionService.LoginUserAsync(request.AccountNumber, request.PinCode);

        if (key == null)
            return Unauthorized();

        return Created(string.Empty, new { SessionKey = key });
    }

    [HttpPost("admin-sessions")]
    public async Task<IActionResult> CreateAdminSession([FromBody] LoginAdminRequest request)
    {
        Guid? key = await _sessionService.LoginAdminAsync(request.Password);

        if (key == null)
            return Unauthorized();

        return Created(string.Empty, new { SessionKey = key });
    }
}