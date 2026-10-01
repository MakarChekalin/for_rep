using Atm.Domain;

namespace Atm.Application;

public class UserProvisioningService : IUserProvisioningService
{
    private readonly IUserRepository _userRepository;

    public UserProvisioningService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task EnsureUserAsync(string userId)
    {
        if (await _userRepository.ExistsAsync(userId))
            return;

        await _userRepository.SaveAsync(new User(userId, DateTime.UtcNow));
    }
}
