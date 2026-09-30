using Atm.Client.Application.DTO;

namespace Atm.Client.Application;

public interface IAtmGateway
{
    Task<Guid?> LoginUserAsync(string accountNumber, string pinCode);

    Task<Guid?> LoginAdminAsync(string password);

    Task<bool> CreateAccountAsync(Guid sessionKey, string number, string pinCode);

    Task<bool> WithdrawAsync(Guid sessionKey, decimal amount);

    Task<bool> DepositAsync(Guid sessionKey, decimal amount);

    Task<decimal?> GetBalanceAsync(Guid sessionKey);

    Task<List<OperationDto>?> GetHistoryAsync(Guid sessionKey);
}