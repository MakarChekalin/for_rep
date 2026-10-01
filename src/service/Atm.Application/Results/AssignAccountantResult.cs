namespace Atm.Application.Results;

public enum AssignAccountantResult
{
    Success,
    Unauthorized,
    NotFound,
    AccountantNotFound,
    InvalidState,
    ServiceUnavailable,
}
