namespace Atm.Application.Results;

public record CreateInvoiceResult(CreateInvoiceStatus Status, Guid? InvoiceId);
