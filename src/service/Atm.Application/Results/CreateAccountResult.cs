namespace Atm.Application.Results;

public enum CreateAccountResult
{
    Success,
    Unauthorized,
    Exists,
    OwnerNotFound,
    AccountLimitExceeded,
}
