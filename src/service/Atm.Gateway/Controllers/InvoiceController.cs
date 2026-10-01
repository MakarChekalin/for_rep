using Atm.Gateway.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atm.Gateway.Controllers;

[ApiController]
[Authorize]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly Atm.Grpc.InvoiceService.InvoiceServiceClient _invoiceClient;

    public InvoiceController(Atm.Grpc.InvoiceService.InvoiceServiceClient invoiceClient)
    {
        _invoiceClient = invoiceClient;
    }

    /// <summary>
    /// Creates an invoice requesting payment from the given payer account, issued by the account
    /// tied to the given user session.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        Atm.Grpc.CreateInvoiceResponse response = await _invoiceClient.CreateInvoiceAsync(new Atm.Grpc.CreateInvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            PayerAccountNumber = request.PayerAccountNumber,
            Amount = AmountFormat.ToGrpc(request.Amount),
            UserId = this.GetUserId(),
        });

        return Created(string.Empty, new { response.InvoiceId });
    }

    /// <summary>
    /// Pays an invoice. Only the payer account can pay it, and only while it is not already paid or cancelled.
    /// </summary>
    [HttpPost("{invoiceId:guid}/pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PayInvoice(Guid invoiceId, [FromBody] InvoiceActionRequest request)
    {
        await _invoiceClient.PayInvoiceAsync(new Atm.Grpc.InvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
            UserId = this.GetUserId(),
        });

        return NoContent();
    }

    /// <summary>
    /// Cancels an invoice. Only the issuing (payee) account can cancel it, and only while it is not already paid.
    /// </summary>
    [HttpPost("{invoiceId:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelInvoice(Guid invoiceId, [FromBody] InvoiceActionRequest request)
    {
        await _invoiceClient.CancelInvoiceAsync(new Atm.Grpc.InvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
            UserId = this.GetUserId(),
        });

        return NoContent();
    }

    /// <summary>
    /// Assigns an accountant to review a corporate-payer invoice. Only the issuing (payee) account can assign one.
    /// </summary>
    [HttpPost("{invoiceId:guid}/accountant")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignAccountant(Guid invoiceId, [FromBody] AssignAccountantRequest request)
    {
        await _invoiceClient.AssignAccountantAsync(new Atm.Grpc.AssignAccountantRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
            UserId = this.GetUserId(),
            AccountantUserId = request.AccountantUserId,
        });

        return NoContent();
    }

    /// <summary>
    /// Approves an invoice as its assigned accountant.
    /// </summary>
    [HttpPost("{invoiceId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveInvoice(Guid invoiceId, [FromBody] InvoiceActionRequest request)
    {
        await _invoiceClient.ApproveInvoiceAsync(new Atm.Grpc.InvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
            UserId = this.GetUserId(),
        });

        return NoContent();
    }

    /// <summary>
    /// Declines an invoice as its assigned accountant.
    /// </summary>
    [HttpPost("{invoiceId:guid}/decline")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeclineInvoice(Guid invoiceId, [FromBody] InvoiceActionRequest request)
    {
        await _invoiceClient.DeclineInvoiceAsync(new Atm.Grpc.InvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
            UserId = this.GetUserId(),
        });

        return NoContent();
    }

    /// <summary>
    /// Returns a page of invoices issued by the account tied to the given user session, optionally
    /// filtered by payer account number and status.
    /// </summary>
    [HttpGet("outgoing")]
    [ProducesResponseType(typeof(GetInvoicesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetOutgoingInvoices(
        [FromQuery] Guid sessionKey,
        [FromQuery] string? payerAccountNumber,
        [FromQuery] string? status,
        [FromQuery] int pageSize = 20,
        [FromQuery] string pageToken = "")
    {
        Atm.Grpc.InvoiceStatus? parsedStatus = null;

        if (status != null)
        {
            if (!Enum.TryParse(status, ignoreCase: true, out Atm.Grpc.InvoiceStatus parsed))
                return BadRequest(new { error = $"Unknown invoice status '{status}'" });

            parsedStatus = parsed;
        }

        var request = new Atm.Grpc.GetOutgoingInvoicesRequest
        {
            SessionKey = sessionKey.ToString(),
            PageSize = pageSize,
            PageToken = pageToken,
            UserId = this.GetUserId(),
        };

        if (payerAccountNumber != null)
            request.PayerAccountNumber = payerAccountNumber;

        if (parsedStatus != null)
            request.Status = parsedStatus.Value;

        Atm.Grpc.GetInvoicesResponse response = await _invoiceClient.GetOutgoingInvoicesAsync(request);

        return Ok(ToResponse(response));
    }

    /// <summary>
    /// Returns a page of invoices to be paid by the account tied to the given user session, optionally
    /// filtered by payee account number and status.
    /// </summary>
    [HttpGet("incoming")]
    [ProducesResponseType(typeof(GetInvoicesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetIncomingInvoices(
        [FromQuery] Guid sessionKey,
        [FromQuery] string? payeeAccountNumber,
        [FromQuery] string? status,
        [FromQuery] int pageSize = 20,
        [FromQuery] string pageToken = "")
    {
        Atm.Grpc.InvoiceStatus? parsedStatus = null;

        if (status != null)
        {
            if (!Enum.TryParse(status, ignoreCase: true, out Atm.Grpc.InvoiceStatus parsed))
                return BadRequest(new { error = $"Unknown invoice status '{status}'" });

            parsedStatus = parsed;
        }

        var request = new Atm.Grpc.GetIncomingInvoicesRequest
        {
            SessionKey = sessionKey.ToString(),
            PageSize = pageSize,
            PageToken = pageToken,
            UserId = this.GetUserId(),
        };

        if (payeeAccountNumber != null)
            request.PayeeAccountNumber = payeeAccountNumber;

        if (parsedStatus != null)
            request.Status = parsedStatus.Value;

        Atm.Grpc.GetInvoicesResponse response = await _invoiceClient.GetIncomingInvoicesAsync(request);

        return Ok(ToResponse(response));
    }

    private static GetInvoicesResponse ToResponse(Atm.Grpc.GetInvoicesResponse response)
    {
        var invoices = response.Invoices.Select(ToInvoiceDto).ToList();
        return new GetInvoicesResponse(invoices, response.NextPageToken);
    }

    private static InvoiceDto ToInvoiceDto(Atm.Grpc.Invoice invoice)
    {
        return new InvoiceDto(
            Guid.Parse(invoice.Id),
            invoice.PayerAccountNumber,
            invoice.PayeeAccountNumber,
            AmountFormat.FromGrpc(invoice.Amount),
            invoice.Status.ToString(),
            invoice.CreatedAt.ToDateTime());
    }
}
