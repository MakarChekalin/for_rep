namespace Atm.Client.Domain;

public interface IAtmGateway
{
    Task<(bool Success, Guid? SessionKey)> LoginUserAsync(string accountNumber, string pinCode);

    Task<(bool Success, Guid? SessionKey)> LoginAdminAsync(string password);

    Task<bool> CreateAccountAsync(Guid sessionKey, string number, string pinCode);

    Task<bool> WithdrawAsync(Guid sessionKey, decimal amount);

    Task<bool> DepositAsync(Guid sessionKey, decimal amount);

    Task<(bool Success, decimal? Balance)> GetBalanceAsync(Guid sessionKey);

    Task<(bool Success, List<OperationResponse>? History)> GetHistoryAsync(Guid sessionKey);
}