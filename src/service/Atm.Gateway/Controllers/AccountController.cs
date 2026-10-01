using Atm.Gateway.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Atm.Gateway.Controllers;

[ApiController]
[Route("api")]
public class AccountController : ControllerBase
{
    private readonly Atm.Grpc.AccountService.AccountServiceClient _accountClient;

    public AccountController(Atm.Grpc.AccountService.AccountServiceClient accountClient)
    {
        _accountClient = accountClient;
    }

    [HttpPost("accounts")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        await _accountClient.CreateAccountAsync(new Atm.Grpc.CreateAccountRequest
        {
            SessionKey = request.SessionKey.ToString(),
            Number = request.Number,
            PinCode = request.PinCode,
        });

        return Created(string.Empty, null);
    }

    [HttpPost("accounts/withdrawals")]
    public async Task<IActionResult> CreateWithdrawal([FromBody] AmountRequest request)
    {
        await _accountClient.WithdrawAsync(new Atm.Grpc.AmountRequest
        {
            SessionKey = request.SessionKey.ToString(),
            Amount = AmountFormat.ToGrpc(request.Amount),
        });

        return Created(string.Empty, null);
    }

    [HttpPost("accounts/deposits")]
    public async Task<IActionResult> CreateDeposit([FromBody] AmountRequest request)
    {
        await _accountClient.DepositAsync(new Atm.Grpc.AmountRequest
        {
            SessionKey = request.SessionKey.ToString(),
            Amount = AmountFormat.ToGrpc(request.Amount),
        });

        return Created(string.Empty, null);
    }

    [HttpGet("accounts/balance")]
    public async Task<IActionResult> GetBalance([FromQuery] Guid sessionKey)
    {
        Atm.Grpc.GetBalanceResponse response = await _accountClient.GetBalanceAsync(new Atm.Grpc.SessionRequest
        {
            SessionKey = sessionKey.ToString(),
        });

        return Ok(new { Balance = AmountFormat.FromGrpc(response.Balance) });
    }

    [HttpGet("accounts/transactions")]
    public async Task<IActionResult> GetHistory([FromQuery] Guid sessionKey, [FromQuery] int pageSize = 20, [FromQuery] string pageToken = "")
    {
        Atm.Grpc.GetHistoryResponse response = await _accountClient.GetHistoryAsync(new Atm.Grpc.GetHistoryRequest
        {
            SessionKey = sessionKey.ToString(),
            PageSize = pageSize,
            PageToken = pageToken,
        });

        var operations = response.Operations.Select(ToOperationDto).ToList();

        return Ok(new GetHistoryResponse(operations, response.NextPageToken));
    }

    private static OperationDto ToOperationDto(Atm.Grpc.OperationHistoryItem item)
    {
        var timestamp = item.Timestamp.ToDateTime();

        return item.KindCase switch
        {
            Atm.Grpc.OperationHistoryItem.KindOneofCase.Withdraw => new OperationDto(
                "Withdraw",
                AmountFormat.FromGrpc(item.Withdraw.Amount),
                item.Withdraw.HasInvoiceId ? Guid.Parse(item.Withdraw.InvoiceId) : null,
                timestamp),
            Atm.Grpc.OperationHistoryItem.KindOneofCase.Deposit => new OperationDto(
                "Deposit",
                AmountFormat.FromGrpc(item.Deposit.Amount),
                item.Deposit.HasInvoiceId ? Guid.Parse(item.Deposit.InvoiceId) : null,
                timestamp),
            Atm.Grpc.OperationHistoryItem.KindOneofCase.None => throw new InvalidOperationException("Operation kind not set"),
            _ => throw new InvalidOperationException("Unknown operation kind"),
        };
    }
}
