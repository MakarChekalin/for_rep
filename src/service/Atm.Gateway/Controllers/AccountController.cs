using Atm.Gateway.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atm.Gateway.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class AccountController : ControllerBase
{
    private readonly Atm.Grpc.AccountService.AccountServiceClient _accountClient;

    public AccountController(Atm.Grpc.AccountService.AccountServiceClient accountClient)
    {
        _accountClient = accountClient;
    }

    /// <summary>
    /// Creates a new account for the given admin session.
    /// </summary>
    [HttpPost("accounts")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        await _accountClient.CreateAccountAsync(new Atm.Grpc.CreateAccountRequest
        {
            SessionKey = request.SessionKey.ToString(),
            Number = request.Number,
            PinCode = request.PinCode,
            UserId = this.GetUserId(),
            OwnerUserId = request.OwnerUserId,
        });

        return Created(string.Empty, null);
    }

    /// <summary>
    /// Withdraws money from the account tied to the given user session.
    /// </summary>
    [HttpPost("accounts/withdrawals")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateWithdrawal([FromBody] AmountRequest request)
    {
        await _accountClient.WithdrawAsync(new Atm.Grpc.AmountRequest
        {
            SessionKey = request.SessionKey.ToString(),
            Amount = AmountFormat.ToGrpc(request.Amount),
            UserId = this.GetUserId(),
            AccountNumber = request.AccountNumber,
        });

        return Created(string.Empty, null);
    }

    /// <summary>
    /// Deposits money into the account tied to the given user session.
    /// </summary>
    [HttpPost("accounts/deposits")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDeposit([FromBody] AmountRequest request)
    {
        await _accountClient.DepositAsync(new Atm.Grpc.AmountRequest
        {
            SessionKey = request.SessionKey.ToString(),
            Amount = AmountFormat.ToGrpc(request.Amount),
            UserId = this.GetUserId(),
            AccountNumber = request.AccountNumber,
        });

        return Created(string.Empty, null);
    }

    /// <summary>
    /// Returns the current balance of the account tied to the given user session.
    /// </summary>
    [HttpGet("accounts/balance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetBalance([FromQuery] Guid sessionKey, [FromQuery] string accountNumber)
    {
        Atm.Grpc.GetBalanceResponse response = await _accountClient.GetBalanceAsync(new Atm.Grpc.SessionRequest
        {
            SessionKey = sessionKey.ToString(),
            UserId = this.GetUserId(),
            AccountNumber = accountNumber,
        });

        return Ok(new { Balance = AmountFormat.FromGrpc(response.Balance) });
    }

    /// <summary>
    /// Returns a page of the operation history for the account tied to the given user session.
    /// Each operation is either a withdrawal or a deposit, distinguished by the "type" field.
    /// </summary>
    [HttpGet("accounts/transactions")]
    [ProducesResponseType(typeof(GetHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetHistory([FromQuery] Guid sessionKey, [FromQuery] string accountNumber, [FromQuery] int pageSize = 20, [FromQuery] string pageToken = "")
    {
        Atm.Grpc.GetHistoryResponse response = await _accountClient.GetHistoryAsync(new Atm.Grpc.GetHistoryRequest
        {
            SessionKey = sessionKey.ToString(),
            PageSize = pageSize,
            PageToken = pageToken,
            UserId = this.GetUserId(),
            AccountNumber = accountNumber,
        });

        var operations = response.Operations.Select(ToOperationDto).ToList();

        return Ok(new GetHistoryResponse(operations, response.NextPageToken));
    }

    private static OperationDto ToOperationDto(Atm.Grpc.OperationHistoryItem item)
    {
        var timestamp = item.Timestamp.ToDateTime();

        return item.KindCase switch
        {
            Atm.Grpc.OperationHistoryItem.KindOneofCase.Withdraw => new WithdrawOperationDto(
                AmountFormat.FromGrpc(item.Withdraw.Amount),
                timestamp,
                item.Withdraw.HasInvoiceId ? Guid.Parse(item.Withdraw.InvoiceId) : null),
            Atm.Grpc.OperationHistoryItem.KindOneofCase.Deposit => new DepositOperationDto(
                AmountFormat.FromGrpc(item.Deposit.Amount),
                timestamp,
                item.Deposit.HasInvoiceId ? Guid.Parse(item.Deposit.InvoiceId) : null),
            Atm.Grpc.OperationHistoryItem.KindOneofCase.None => throw new InvalidOperationException("Operation kind not set"),
            _ => throw new InvalidOperationException("Unknown operation kind"),
        };
    }
}
