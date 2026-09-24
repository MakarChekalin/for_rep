using Atm.Client.Domain;

namespace Atm.Client.Application;

public interface IAtmClientService
{
    Task<bool> LoginUserAsync(string accountNumber, string pinCode);

    Task<bool> LoginAdminAsync(string password);

    Task<CreateAccountResult> CreateAccountAsync(string number, string pinCode);

    Task<(bool Success, string? Error)> WithdrawAsync(decimal amount);

    Task<(bool Success, string? Error)> DepositAsync(decimal amount);

    Task<(bool Success, decimal? Balance)> GetBalanceAsync();

    Task<(bool Success, List<OperationResponse>? History)> GetHistoryAsync();
}