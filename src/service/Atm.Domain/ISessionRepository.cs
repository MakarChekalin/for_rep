namespace Atm.Domain;

public interface ISessionRepository
{
    Task<Session?> GetByKeyAsync(Guid key);
    Task SaveAsync(Session session);
}