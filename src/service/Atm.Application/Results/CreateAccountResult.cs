namespace Atm.Application.Results;

public enum CreateAccountResult
{
    Success,
    NotLoggedIn,
    Unauthorized,
    Exists,
    Error,
}