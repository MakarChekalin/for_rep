using Atm.Application;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi.Controllers;

[ApiController]
[Route("api")]
public class SessionController : ControllerBase
{
    private readonly SessionService _sessionService;

    public SessionController(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public record LoginUserRequest(
        [Required] string AccountNumber,
        [Required] string PinCode);

    public record LoginAdminRequest(
        [Required] string Password);

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