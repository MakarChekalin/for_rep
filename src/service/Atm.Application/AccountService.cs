using Atm.Application.Results;
using Atm.Domain;

namespace Atm.Application;

public class AccountService : IAccountService
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

    public async Task<CreateAccountResult> CreateAccountAsync(Guid sessionKey, string number, string pinCode)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.Admin)
            return CreateAccountResult.Unauthorized;

        if (await _accountRepository.ExistsAsync(number))
            return CreateAccountResult.Exists;

        var account = new Account(number, pinCode, balance: 0);
        await _accountRepository.SaveAsync(account);

        return CreateAccountResult.Success;
    }

    public async Task<WithdrawResult> WithdrawAsync(Guid sessionKey, decimal amount)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return WithdrawResult.Unauthorized;

        Account? account = await _accountRepository.GetByNumberAsync(session.AccountNumber);

        if (account == null)
            return WithdrawResult.Unauthorized;

        bool success = account.Withdraw(amount);

        if (!success)
            return WithdrawResult.InsufficientFunds;

        await _accountRepository.SaveAsync(account);
        await _operationRepository.SaveAsync(new Operation(account.Number, OperationType.Withdraw, amount));

        return WithdrawResult.Success;
    }

    public async Task<DepositResult> DepositAsync(Guid sessionKey, decimal amount)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return DepositResult.Unauthorized;

        Account? account = await _accountRepository.GetByNumberAsync(session.AccountNumber);

        if (account == null)
            return DepositResult.Unauthorized;

        account.Deposit(amount);

        await _accountRepository.SaveAsync(account);
        await _operationRepository.SaveAsync(new Operation(account.Number, OperationType.Deposit, amount));

        return DepositResult.Success;
    }

    public async Task<GetBalanceResult> GetBalanceAsync(Guid sessionKey)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return new GetBalanceResult(GetBalanceStatus.Unauthorized, 0);

        Account? account = await _accountRepository.GetByNumberAsync(session.AccountNumber);

        if (account == null)
            return new GetBalanceResult(GetBalanceStatus.Unauthorized, 0);

        return new GetBalanceResult(GetBalanceStatus.Success, account.Balance);
    }

    public async Task<GetHistoryResult> GetHistoryAsync(Guid sessionKey, long? cursor, int pageSize)
    {
        Session? session = await _sessionRepository.GetByKeyAsync(sessionKey);

        if (session == null || session.Type != SessionType.User || session.AccountNumber == null)
            return new GetHistoryResult(GetHistoryStatus.Unauthorized, null, null);

        var operations = new List<Operation>();

        await foreach (Operation operation in _operationRepository.GetByAccountNumberAsync(session.AccountNumber, cursor, pageSize + 1))
            operations.Add(operation);

        long? nextCursor = null;

        if (operations.Count > pageSize)
        {
            nextCursor = operations[pageSize - 1].Id;
            operations.RemoveRange(pageSize, operations.Count - pageSize);
        }

        return new GetHistoryResult(GetHistoryStatus.Success, operations, nextCursor);
    }
}
