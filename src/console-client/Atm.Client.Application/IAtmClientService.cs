using Atm.Client.Application.DTO;
using Atm.Client.Domain;

namespace Atm.Client.Application;

public interface IAtmClientService
{
    Task<LoginResult> LoginUserAsync(string accountNumber, string pinCode);

    Task<LoginResult> LoginAdminAsync(string password);

    Task<CreateAccountResult> CreateAccountAsync(string number, string pinCode);

    Task<WithdrawResult> WithdrawAsync(decimal amount);

    Task<DepositResult> DepositAsync(decimal amount);

    Task<decimal?> GetBalanceAsync();

    Task<List<OperationDto>?> GetHistoryAsync();
}