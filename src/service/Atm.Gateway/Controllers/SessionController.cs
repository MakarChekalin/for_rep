using Atm.Gateway.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Atm.Gateway.Controllers;

[ApiController]
[Route("api")]
public class SessionController : ControllerBase
{
    private readonly Atm.Grpc.SessionService.SessionServiceClient _sessionClient;

    public SessionController(Atm.Grpc.SessionService.SessionServiceClient sessionClient)
    {
        _sessionClient = sessionClient;
    }

    /// <summary>
    /// Logs in a user by account number and pin code and starts a new session.
    /// </summary>
    [HttpPost("user-sessions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateUserSession([FromBody] LoginUserRequest request)
    {
        var grpcRequest = new Atm.Grpc.LoginUserRequest
        {
            AccountNumber = request.AccountNumber,
            PinCode = request.PinCode,
        };

        Atm.Grpc.LoginResponse response = await _sessionClient.LoginUserAsync(grpcRequest);

        return Created(string.Empty, new { response.SessionKey });
    }

    /// <summary>
    /// Logs in as admin by password and starts a new session.
    /// </summary>
    [HttpPost("admin-sessions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAdminSession([FromBody] LoginAdminRequest request)
    {
        var grpcRequest = new Atm.Grpc.LoginAdminRequest
        {
            Password = request.Password,
        };

        Atm.Grpc.LoginResponse response = await _sessionClient.LoginAdminAsync(grpcRequest);

        return Created(string.Empty, new { response.SessionKey });
    }
}
