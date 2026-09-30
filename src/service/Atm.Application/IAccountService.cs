using Atm.Application.Results;

namespace Atm.Application;

public interface IAccountService
{
    Task<CreateAccountResult> CreateAccountAsync(Guid sessionKey, string number, string pinCode);

    Task<WithdrawResult> WithdrawAsync(Guid sessionKey, decimal amount);

    Task<DepositResult> DepositAsync(Guid sessionKey, decimal amount);

    Task<GetBalanceResult> GetBalanceAsync(Guid sessionKey);

    Task<GetHistoryResult> GetHistoryAsync(Guid sessionKey);
}