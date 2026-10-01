using Atm.Application.Results;

namespace Atm.Application;

public interface IAccountService
{
    Task<CreateAccountResult> CreateAccountAsync(Guid sessionKey, string number, string pinCode, string ownerUserId);

    Task<WithdrawResult> WithdrawAsync(Guid sessionKey, decimal amount, string accountNumber, string userId);

    Task<DepositResult> DepositAsync(Guid sessionKey, decimal amount, string accountNumber, string userId);

    Task<GetBalanceResult> GetBalanceAsync(Guid sessionKey, string accountNumber, string userId);

    Task<GetHistoryResult> GetHistoryAsync(Guid sessionKey, string accountNumber, string userId, long? cursor, int pageSize);
}