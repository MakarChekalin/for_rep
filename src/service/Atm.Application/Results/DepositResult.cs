namespace Atm.Application.Results;

public enum DepositResult
{
    Success,
    NotLoggedIn,
    Unauthorized,
    Exists,
    Error,
}