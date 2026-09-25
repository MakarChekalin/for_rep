using Atm.Client.Domain;

namespace Atm.Client.Application;

public class AtmClientService : IAtmClientService
{
    private readonly IAtmGateway _gateway;
    private readonly ClientSessionState _sessionState;

    public AtmClientService(IAtmGateway gateway, ClientSessionState sessionState)
    {
        _gateway = gateway;
        _sessionState = sessionState;
    }

    public async Task<LoginResult> LoginUserAsync(string accountNumber, string pinCode)
    {
        Guid? sessionKey = await _gateway.LoginUserAsync(accountNumber, pinCode);

        if (sessionKey == null)
            return LoginResult.Error;

        _sessionState.SessionKey = sessionKey;
        return LoginResult.Success;
    }

    public async Task<LoginResult> LoginAdminAsync(string password)
    {
        Guid? sessionKey = await _gateway.LoginAdminAsync(password);

        if (sessionKey == null)
            return LoginResult.Error;

        _sessionState.SessionKey = sessionKey;
        return LoginResult.Success;
    }

    public async Task<CreateAccountResult> CreateAccountAsync(string number, string pinCode)
    {
        if (_sessionState.SessionKey == null)
            return CreateAccountResult.NotLoggedIn;

        var success = await _gateway.CreateAccountAsync(_sessionState.SessionKey.Value, number, pinCode);
        return success ? CreateAccountResult.Success : CreateAccountResult.Error;
    }

    public async Task<WithdrawResult> WithdrawAsync(decimal amount)
    {
        if (_sessionState.SessionKey == null)
            return WithdrawResult.NotLoggedIn;

        bool success = await _gateway.WithdrawAsync(_sessionState.SessionKey.Value, amount);
        return success ? WithdrawResult.Success : WithdrawResult.Error;
    }

    public async Task<DepositResult> DepositAsync(decimal amount)
    {
        if (_sessionState.SessionKey == null)
            return DepositResult.NotLoggedIn;

        bool success = await _gateway.DepositAsync(_sessionState.SessionKey.Value, amount);
        return success ? DepositResult.Success : DepositResult.Error;
    }

    public async Task<decimal?> GetBalanceAsync()
    {
        if (_sessionState.SessionKey == null)
            return null;

        return await _gateway.GetBalanceAsync(_sessionState.SessionKey.Value);
    }

    public async Task<List<OperationResponse>?> GetHistoryAsync()
    {
        if (_sessionState.SessionKey == null)
            return null;

        return await _gateway.GetHistoryAsync(_sessionState.SessionKey.Value);
    }
}
