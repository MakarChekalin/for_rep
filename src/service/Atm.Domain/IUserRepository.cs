namespace Atm.Domain;

public interface IUserRepository
{
    Task<bool> ExistsAsync(string id);

    Task<bool> ExistsByExternalIdAsync(long externalId);

    Task<long?> GetExternalIdAsync(string id);

    Task SaveAsync(User user);
}
