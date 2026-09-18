using Atm.Domain;

namespace Atm.Application;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IOperationRepository _operationRepository;

    public AccountService(
        IAccountRepository accountRepository,
        ISessionRepository sessionRepository,
        IOperationRepository operationRepository)
    {
        _accountRepository = accountRepository;
        _sessionRepository = sessionRepository;
        _operationRepository = operationRepository;
    }

    public async Task<bool> CreateAccountAsync(Guid sessionKey, string number, string pinCode)
    {
        var session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.Admin)
            return false;

        if (await _accountRepository.ExistsAsync(number))
            return false;

        var account = new Account(number, pinCode, balance: 0);
        await _accountRepository.SaveAsync(account);

        return true;
    }

    public async Task<(bool Success, string? Error)> WithdrawAsync(Guid sessionKey, decimal amount)
    {
        var session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return (false, "Unauthorized");

        var account = await _accountRepository.GetByNumberAsync(session.AccountNumber);

        if (account == null)
            return (false, "Unauthorized");

        var success = account.Withdraw(amount);

        if (!success)
            return (false, "Insufficient funds");

        await _accountRepository.SaveAsync(account);
        await _operationRepository.SaveAsync(new Operation(account.Number, OperationType.Withdraw, amount));

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DepositAsync(Guid sessionKey, decimal amount)
    {
        var session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return (false, "Unauthorized");

        var account = await _accountRepository.GetByNumberAsync(session.AccountNumber);

        if (account == null)
            return (false, "Unauthorized");

        account.Deposit(amount);

        await _accountRepository.SaveAsync(account);
        await _operationRepository.SaveAsync(new Operation(account.Number, OperationType.Deposit, amount));

        return (true, null);
    }

    public async Task<(bool Success, decimal? Balance)> GetBalanceAsync(Guid sessionKey)
    {
        var session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return (false, null);

        var account = await _accountRepository.GetByNumberAsync(session.AccountNumber);

        if (account == null)
            return (false, null);

        return (true, account.Balance);
    }

    public async Task<(bool Success, IReadOnlyList<Operation>? History)> GetHistoryAsync(Guid sessionKey)
    {
        var session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return (false, null);

        var history = await _operationRepository.GetByAccountNumberAsync(session.AccountNumber);

        return (true, history);
    }
}