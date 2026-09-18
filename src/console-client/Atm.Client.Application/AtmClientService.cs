using Atm.Client.Domain;

namespace Atm.Client.Application;

public class AtmClientService
{
    private readonly IAtmGateway _gateway;
    private readonly SessionState _sessionState;

    public AtmClientService(IAtmGateway gateway, SessionState sessionState)
    {
        _gateway = gateway;
        _sessionState = sessionState;
    }

    public async Task<bool> LoginUserAsync(string accountNumber, string pinCode)
    {
        var (success, sessionKey) = await _gateway.LoginUserAsync(accountNumber, pinCode);

        if (!success || sessionKey == null)
            return false;

        _sessionState.SessionKey = sessionKey;
        return true;
    }

    public async Task<bool> LoginAdminAsync(string password)
    {
        var (success, sessionKey) = await _gateway.LoginAdminAsync(password);

        if (!success || sessionKey == null)
            return false;

        _sessionState.SessionKey = sessionKey;
        return true;
    }

    public async Task<(bool Success, string? Error)> CreateAccountAsync(string number, string pinCode)
    {
        if (_sessionState.SessionKey == null)
            return (false, "Not logged in");

        var success = await _gateway.CreateAccountAsync(_sessionState.SessionKey.Value, number, pinCode);
        return (success, success ? null : "Failed to create account");
    }

    public async Task<(bool Success, string? Error)> WithdrawAsync(decimal amount)
    {
        if (_sessionState.SessionKey == null)
            return (false, "Not logged in");

        var success = await _gateway.WithdrawAsync(_sessionState.SessionKey.Value, amount);
        return (success, success ? null : "Withdrawal failed");
    }

    public async Task<(bool Success, string? Error)> DepositAsync(decimal amount)
    {
        if (_sessionState.SessionKey == null)
            return (false, "Not logged in");

        var success = await _gateway.DepositAsync(_sessionState.SessionKey.Value, amount);
        return (success, success ? null : "Deposit failed");
    }

    public async Task<(bool Success, decimal? Balance)> GetBalanceAsync()
    {
        if (_sessionState.SessionKey == null)
            return (false, null);

        return await _gateway.GetBalanceAsync(_sessionState.SessionKey.Value);
    }

    public async Task<(bool Success, List<OperationResponse>? History)> GetHistoryAsync()
    {
        if (_sessionState.SessionKey == null)
            return (false, null);

        return await _gateway.GetHistoryAsync(_sessionState.SessionKey.Value);
    }
}