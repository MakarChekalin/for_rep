using Atm.Application;
using Atm.Application.Results;
using Atm.WebApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Atm.WebApi.Controllers;

[ApiController]
[Route("api")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        CreateAccountResult result = await _accountService.CreateAccountAsync(request.SessionKey, request.Number, request.PinCode);

        if (result != CreateAccountResult.Success)
            return BadRequest();

        return Created(string.Empty, null); // 201 - успешно создано что то
    }

    [HttpPost("accounts/withdrawals")]
    public async Task<IActionResult> CreateWithdrawal([FromBody] AmountRequest request)
    {
        WithdrawResult result = await _accountService.WithdrawAsync(request.SessionKey, request.Amount);

        if (result == WithdrawResult.Unauthorized)
            return Unauthorized();

        if (result != WithdrawResult.Success)
            return BadRequest(result.ToString());

        return Created(string.Empty, null);
    }

    [HttpPost("accounts/deposits")]
    public async Task<IActionResult> CreateDeposit([FromBody] AmountRequest request)
    {
        DepositResult result = await _accountService.DepositAsync(request.SessionKey, request.Amount);

        if (result == DepositResult.Unauthorized)
            return Unauthorized();

        if (result != DepositResult.Success)
            return BadRequest(result.ToString());

        return Created(string.Empty, null);
    }

    [HttpGet("accounts/balance")] // сделал GET так как по REST
    public async Task<IActionResult> GetBalance([FromQuery] Guid sessionKey) // FromQuery - передаем сессионный ключ прям в url
    {
        GetBalanceResult result = await _accountService.GetBalanceAsync(sessionKey);

        if (result.Status != GetBalanceStatus.Success)
            return Unauthorized();

        return Ok(new { result.Balance });
    }

    [HttpGet("accounts/transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] Guid sessionKey)
    {
        GetHistoryResult result = await _accountService.GetHistoryAsync(sessionKey);

        if (result.Status != GetHistoryStatus.Success)
            return Unauthorized();

        return Ok(result.History);
    }
}