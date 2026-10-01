namespace Atm.Application.Results;

public enum CancelInvoiceResult
{
    Success,
    Unauthorized,
    NotFound,
    AlreadyProcessed,
    PendingApproval,
}
