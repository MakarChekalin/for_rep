namespace Atm.Client.Domain;

public enum CreateAccountResult
{
    Success,
    NotLoggedIn,
    Unauthorized,
    Exists,
    Error,
}