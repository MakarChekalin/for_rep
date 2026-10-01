using Atm.Application;
using Atm.Domain;
using Moq;

namespace Atm.Tests;

public class UserProvisioningServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();

    [Fact]
    public async Task EnsureUser_WhenUserDoesNotExist_ShouldCreateUser()
    {
        _userRepository.Setup(r => r.ExistsAsync("user-1")).ReturnsAsync(false);

        UserProvisioningService service = CreateService();

        await service.EnsureUserAsync("user-1");

        _userRepository.Verify(r => r.SaveAsync(It.Is<User>(u => u.Id == "user-1")), Times.Once);
    }

    [Fact]
    public async Task EnsureUser_WhenUserAlreadyExists_ShouldNotSaveAgain()
    {
        _userRepository.Setup(r => r.ExistsAsync("user-1")).ReturnsAsync(true);

        UserProvisioningService service = CreateService();

        await service.EnsureUserAsync("user-1");

        _userRepository.Verify(r => r.SaveAsync(It.IsAny<User>()), Times.Never);
    }

    private UserProvisioningService CreateService()
    {
        return new UserProvisioningService(_userRepository.Object);
    }
}
