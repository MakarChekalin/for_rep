namespace Atm.Domain;

public interface IUserRepository
{
    Task<bool> ExistsAsync(string id);

    Task SaveAsync(User user);
}
