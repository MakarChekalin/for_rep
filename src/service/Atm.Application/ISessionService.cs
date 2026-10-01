using Atm.Domain;

namespace Atm.Application;

public interface ISessionService
{
    Task<Guid?> LoginUserAsync(string accountNumber, string pinCode);

    Task<Guid?> LoginAdminAsync(string password);

    Task<Session?> ValidateAsync(Guid sessionKey);
}