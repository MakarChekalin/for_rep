using Atm.Client.Domain;
using Refit;

namespace Atm.Client.Infrastructure;

public class RefitAtmGateway : IAtmGateway
{
    private readonly IAtmApi _api;

    public RefitAtmGateway(IAtmApi api)
    {
        _api = api;
    }

    public async Task<Guid?> LoginUserAsync(string accountNumber, string pinCode)
    {
        IApiResponse<SessionKeyResponse> response = await _api.CreateUserSessionAsync(new LoginUserBody(accountNumber, pinCode));

        if (!response.IsSuccessStatusCode || response.Content == null)
            return null;

        return response.Content.SessionKey;
    }

    public async Task<Guid?> LoginAdminAsync(string password)
    {
        IApiResponse<SessionKeyResponse> response = await _api.CreateAdminSessionAsync(new LoginAdminBody(password));

        if (!response.IsSuccessStatusCode || response.Content == null)
            return null;

        return response.Content.SessionKey;
    }

    public async Task<bool> CreateAccountAsync(Guid sessionKey, string number, string pinCode)
    {
        IApiResponse response = await _api.CreateAccountAsync(new CreateAccountBody(sessionKey, number, pinCode));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> WithdrawAsync(Guid sessionKey, decimal amount)
    {
        IApiResponse response = await _api.WithdrawAsync(new AmountBody(sessionKey, amount));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DepositAsync(Guid sessionKey, decimal amount)
    {
        IApiResponse response = await _api.DepositAsync(new AmountBody(sessionKey, amount));
        return response.IsSuccessStatusCode;
    }

    public async Task<decimal?> GetBalanceAsync(Guid sessionKey)
    {
        IApiResponse<BalanceResponse> response = await _api.GetBalanceAsync(sessionKey);

        if (!response.IsSuccessStatusCode || response.Content == null)
            return null;

        return response.Content.Balance;
    }

    public async Task<List<OperationResponse>?> GetHistoryAsync(Guid sessionKey)
    {
        IApiResponse<List<OperationResponse>> response = await _api.GetTransactionsAsync(sessionKey);

        if (!response.IsSuccessStatusCode || response.Content == null)
            return null;

        return response.Content;
    }
}