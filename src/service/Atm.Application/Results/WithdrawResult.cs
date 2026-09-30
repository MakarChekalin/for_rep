namespace Atm.Application.Results;

public enum WithdrawResult
{
    Success,
    NotLoggedIn,
    Unauthorized,
    InsufficientFunds,
    Exists,
    Error,
}