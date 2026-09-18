using Atm.Client.Domain;
using Refit;

namespace Atm.Client.Infrastructure;

public interface IAtmApi
{
    [Post("/api/user-sessions")]
    Task<IApiResponse<SessionKeyResponse>> CreateUserSessionAsync([Body] LoginUserBody body);

    [Post("/api/admin-sessions")]
    Task<IApiResponse<SessionKeyResponse>> CreateAdminSessionAsync([Body] LoginAdminBody body);

    [Post("/api/accounts")]
    Task<IApiResponse> CreateAccountAsync([Body] CreateAccountBody body);

    [Post("/api/accounts/withdrawals")]
    Task<IApiResponse> WithdrawAsync([Body] AmountBody body);

    [Post("/api/accounts/deposits")]
    Task<IApiResponse> DepositAsync([Body] AmountBody body);

    [Get("/api/accounts/balance")]
    Task<IApiResponse<BalanceResponse>> GetBalanceAsync([Query] Guid sessionKey);

    [Get("/api/accounts/transactions")]
    Task<IApiResponse<List<OperationResponse>>> GetTransactionsAsync([Query] Guid sessionKey);
}

public record LoginUserBody(string AccountNumber, string PinCode);
public record LoginAdminBody(string Password);
public record CreateAccountBody(Guid SessionKey, string Number, string PinCode);
public record AmountBody(Guid SessionKey, decimal Amount);