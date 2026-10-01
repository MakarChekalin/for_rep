namespace Atm.Gateway.DTO;

public record GetInvoicesResponse(IReadOnlyList<InvoiceDto> Invoices, string NextPageToken);
