using Atm.Application;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Atm.WebApi.Controllers;

[ApiController]
[Route("api")]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    public record CreateAccountRequest(
        [Required] Guid SessionKey,
        [Required] string Number,
        [Required] string PinCode);

    public record AmountRequest(
        [Required] Guid SessionKey,
        [Range(0.01, double.MaxValue)] decimal Amount); // чтобы не снимали <0

    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var success = await _accountService.CreateAccountAsync(request.SessionKey, request.Number, request.PinCode);

        if (!success)
            return BadRequest();

        return Created(string.Empty, null); // 201 - успешно создано что то
    }

    [HttpPost("accounts/withdrawals")]
    public async Task<IActionResult> CreateWithdrawal([FromBody] AmountRequest request)
    {
        var (success, error) = await _accountService.WithdrawAsync(request.SessionKey, request.Amount);

        if (!success)
        {
            if (error == "Unauthorized")
                return Unauthorized();

            return BadRequest(error);
        }

        return Created(string.Empty, null);
    }

    [HttpPost("accounts/deposits")]
    public async Task<IActionResult> CreateDeposit([FromBody] AmountRequest request)
    {
        var (success, error) = await _accountService.DepositAsync(request.SessionKey, request.Amount);

        if (!success)
        {
            if (error == "Unauthorized")
                return Unauthorized();

            return BadRequest(error);
        }

        return Created(string.Empty, null);
    }

    [HttpGet("accounts/balance")] // сделал GET так как по REST
    public async Task<IActionResult> GetBalance([FromQuery] Guid sessionKey) // FromQuery - передаем сессионный ключ прям в url
    {
        var (success, balance) = await _accountService.GetBalanceAsync(sessionKey);

        if (!success)
            return Unauthorized();

        return Ok(new { Balance = balance });
    }

    [HttpGet("accounts/transactions")]
    public async Task<IActionResult> GetTransactions([FromQuery] Guid sessionKey)
    {
        var (success, history) = await _accountService.GetHistoryAsync(sessionKey);

        if (!success)
            return Unauthorized();

        return Ok(history);
    }
}