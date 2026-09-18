using Atm.Domain;

namespace Atm.Application;

public class SessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly string _adminPassword;

    public SessionService(
        ISessionRepository sessionRepository,
        IAccountRepository accountRepository,
        string adminPassword)
    {
        _sessionRepository = sessionRepository;
        _accountRepository = accountRepository;
        _adminPassword = adminPassword;
    }

    public async Task<Guid?> LoginUserAsync(string accountNumber, string pinCode)
    {
        var account = await _accountRepository.GetByNumberAsync(accountNumber);

        if (account == null)
            return null;

        if (account.PinCode != pinCode)
            return null;

        var session = new Session(SessionType.User, accountNumber);
        await _sessionRepository.SaveAsync(session);

        return session.Key;
    }

    public async Task<Guid?> LoginAdminAsync(string password)
    {
        if (password != _adminPassword)
            return null;

        var session = new Session(SessionType.Admin);
        await _sessionRepository.SaveAsync(session);

        return session.Key;
    }

    public async Task<Session?> ValidateAsync(Guid sessionKey)
    {
        return await _sessionRepository.GetByKeyAsync(sessionKey);
    }
}