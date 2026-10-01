namespace Atm.Application;

public interface IUserProvisioningService
{
    Task EnsureUserAsync(string userId);
}
