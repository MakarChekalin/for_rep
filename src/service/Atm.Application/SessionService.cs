using Atm.Domain;

namespace Atm.Application;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IAdminPasswordValidator _adminPasswordValidator;

    public SessionService(
        ISessionRepository sessionRepository,
        IAccountRepository accountRepository,
        IAdminPasswordValidator adminPasswordValidator)
    {
        _sessionRepository = sessionRepository;
        _accountRepository = accountRepository;
        _adminPasswordValidator = adminPasswordValidator;
    }

    public async Task<Guid?> LoginUserAsync(string accountNumber, string pinCode)
    {
        Account? account = await _accountRepository.GetByNumberAsync(accountNumber);

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
        if (!_adminPasswordValidator.IsValid(password))
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