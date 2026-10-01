using Atm.Gateway.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Atm.Gateway.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoiceController : ControllerBase
{
    private readonly Atm.Grpc.InvoiceService.InvoiceServiceClient _invoiceClient;

    public InvoiceController(Atm.Grpc.InvoiceService.InvoiceServiceClient invoiceClient)
    {
        _invoiceClient = invoiceClient;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        Atm.Grpc.CreateInvoiceResponse response = await _invoiceClient.CreateInvoiceAsync(new Atm.Grpc.CreateInvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            PayerAccountNumber = request.PayerAccountNumber,
            Amount = AmountFormat.ToGrpc(request.Amount),
        });

        return Created(string.Empty, new { response.InvoiceId });
    }

    [HttpPost("{invoiceId:guid}/pay")]
    public async Task<IActionResult> PayInvoice(Guid invoiceId, [FromBody] InvoiceActionRequest request)
    {
        await _invoiceClient.PayInvoiceAsync(new Atm.Grpc.InvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
        });

        return NoContent();
    }

    [HttpPost("{invoiceId:guid}/cancel")]
    public async Task<IActionResult> CancelInvoice(Guid invoiceId, [FromBody] InvoiceActionRequest request)
    {
        await _invoiceClient.CancelInvoiceAsync(new Atm.Grpc.InvoiceRequest
        {
            SessionKey = request.SessionKey.ToString(),
            InvoiceId = invoiceId.ToString(),
        });

        return NoContent();
    }

    [HttpGet("outgoing")]
    public async Task<IActionResult> GetOutgoingInvoices(
        [FromQuery] Guid sessionKey,
        [FromQuery] string? payerAccountNumber,
        [FromQuery] string? status,
        [FromQuery] int pageSize = 20,
        [FromQuery] string pageToken = "")
    {
        var request = new Atm.Grpc.GetOutgoingInvoicesRequest
        {
            SessionKey = sessionKey.ToString(),
            PageSize = pageSize,
            PageToken = pageToken,
        };

        if (payerAccountNumber != null)
            request.PayerAccountNumber = payerAccountNumber;

        if (status != null)
            request.Status = ParseStatus(status);

        Atm.Grpc.GetInvoicesResponse response = await _invoiceClient.GetOutgoingInvoicesAsync(request);

        return Ok(ToResponse(response));
    }

    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncomingInvoices(
        [FromQuery] Guid sessionKey,
        [FromQuery] string? payeeAccountNumber,
        [FromQuery] string? status,
        [FromQuery] int pageSize = 20,
        [FromQuery] string pageToken = "")
    {
        var request = new Atm.Grpc.GetIncomingInvoicesRequest
        {
            SessionKey = sessionKey.ToString(),
            PageSize = pageSize,
            PageToken = pageToken,
        };

        if (payeeAccountNumber != null)
            request.PayeeAccountNumber = payeeAccountNumber;

        if (status != null)
            request.Status = ParseStatus(status);

        Atm.Grpc.GetInvoicesResponse response = await _invoiceClient.GetIncomingInvoicesAsync(request);

        return Ok(ToResponse(response));
    }

    private static Atm.Grpc.InvoiceStatus ParseStatus(string status)
    {
        if (Enum.TryParse(status, ignoreCase: true, out Atm.Grpc.InvoiceStatus result))
            return result;

        throw new ArgumentException($"Unknown invoice status '{status}'", nameof(status));
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
