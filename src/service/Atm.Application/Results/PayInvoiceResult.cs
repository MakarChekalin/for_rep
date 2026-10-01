namespace Atm.Application.Results;

public enum PayInvoiceResult
{
    Success,
    Unauthorized,
    NotFound,
    AlreadyProcessed,
    InsufficientFunds,
}
