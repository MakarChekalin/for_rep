using Atm.Client.Domain;

namespace Atm.Client.Infrastructure;

public class RefitAtmGateway : IAtmGateway
{
    private readonly IAtmApi _api;

    public RefitAtmGateway(IAtmApi api)
    {
        _api = api;
    }

    public async Task<(bool Success, Guid? SessionKey)> LoginUserAsync(string accountNumber, string pinCode)
    {
        var response = await _api.CreateUserSessionAsync(new LoginUserBody(accountNumber, pinCode));

        if (!response.IsSuccessStatusCode || response.Content == null)
            return (false, null);

        return (true, response.Content.SessionKey);
    }

    public async Task<(bool Success, Guid? SessionKey)> LoginAdminAsync(string password)
    {
        var response = await _api.CreateAdminSessionAsync(new LoginAdminBody(password));

        if (!response.IsSuccessStatusCode || response.Content == null)
            return (false, null);

        return (true, response.Content.SessionKey);
    }

    public async Task<bool> CreateAccountAsync(Guid sessionKey, string number, string pinCode)
    {
        var response = await _api.CreateAccountAsync(new CreateAccountBody(sessionKey, number, pinCode));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> WithdrawAsync(Guid sessionKey, decimal amount)
    {
        var response = await _api.WithdrawAsync(new AmountBody(sessionKey, amount));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DepositAsync(Guid sessionKey, decimal amount)
    {
        var response = await _api.DepositAsync(new AmountBody(sessionKey, amount));
        return response.IsSuccessStatusCode;
    }

    public async Task<(bool Success, decimal? Balance)> GetBalanceAsync(Guid sessionKey)
    {
        var response = await _api.GetBalanceAsync(sessionKey);

        if (!response.IsSuccessStatusCode || response.Content == null)
            return (false, null);

        return (true, response.Content.Balance);
    }

    public async Task<(bool Success, List<OperationResponse>? History)> GetHistoryAsync(Guid sessionKey)
    {
        var response = await _api.GetTransactionsAsync(sessionKey);

        if (!response.IsSuccessStatusCode || response.Content == null)
         return (false, null);

      return (true, response.Content);
    }
}