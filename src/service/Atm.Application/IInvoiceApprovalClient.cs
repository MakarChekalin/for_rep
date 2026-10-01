namespace Atm.Application;

public enum InvoiceApprovalCallResult
{
    Success,
    NotFound,
    InvalidState,
    ServiceUnavailable,
}

public interface IInvoiceApprovalClient
{
    Task<InvoiceApprovalCallResult> AssignAccountantAsync(long invoiceExternalId, long accountantExternalId, CancellationToken cancellationToken);

    Task<InvoiceApprovalCallResult> ApproveInvoiceAsync(long invoiceExternalId, long accountantExternalId, CancellationToken cancellationToken);

    Task<InvoiceApprovalCallResult> DeclineInvoiceAsync(long invoiceExternalId, long accountantExternalId, CancellationToken cancellationToken);
}
