using Atm.Application;
using Atm.Domain;
using FluentAssertions;
using Moq;

namespace Atm.Tests;

public class SessionServiceTests
{
    private readonly Mock<ISessionRepository> _sessionRepository = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly Mock<IAdminPasswordValidator> _adminPasswordValidator = new();

    [Fact]
    public async Task LoginUser_WithCorrectPin_ShouldCreateUserSession()
    {
        var account = new Account("40817123", "1234", "user-1", 100);
        _accountRepository.Setup(r => r.GetByNumberAsync("40817123")).ReturnsAsync(account);

        SessionService service = CreateService();

        Guid? key = await service.LoginUserAsync("40817123", "1234");

        key.Should().NotBeNull();
        _sessionRepository.Verify(
            r => r.SaveAsync(It.Is<Session>(s => s.Type == SessionType.User && s.AccountNumber == "40817123" && s.Key == key)),
            Times.Once);
    }

    [Fact]
    public async Task LoginUser_WithUnknownAccount_ShouldReturnNull()
    {
        _accountRepository.Setup(r => r.GetByNumberAsync(It.IsAny<string>())).ReturnsAsync((Account?)null);

        SessionService service = CreateService();

        Guid? key = await service.LoginUserAsync("40817123", "1234");

        key.Should().BeNull();
        _sessionRepository.Verify(r => r.SaveAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task LoginUser_WithWrongPin_ShouldReturnNull()
    {
        var account = new Account("40817123", "1234", "user-1", 100);
        _accountRepository.Setup(r => r.GetByNumberAsync("40817123")).ReturnsAsync(account);

        SessionService service = CreateService();

        Guid? key = await service.LoginUserAsync("40817123", "wrong-pin");

        key.Should().BeNull();
        _sessionRepository.Verify(r => r.SaveAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task LoginAdmin_WithCorrectPassword_ShouldCreateAdminSession()
    {
        _adminPasswordValidator.Setup(v => v.IsValid("correct-password")).Returns(true);

        SessionService service = CreateService();

        Guid? key = await service.LoginAdminAsync("correct-password");

        key.Should().NotBeNull();
        _sessionRepository.Verify(
            r => r.SaveAsync(It.Is<Session>(s => s.Type == SessionType.Admin && s.AccountNumber == null)),
            Times.Once);
    }

    [Fact]
    public async Task LoginAdmin_WithWrongPassword_ShouldReturnNull()
    {
        _adminPasswordValidator.Setup(v => v.IsValid(It.IsAny<string>())).Returns(false);

        SessionService service = CreateService();

        Guid? key = await service.LoginAdminAsync("wrong-password");

        key.Should().BeNull();
        _sessionRepository.Verify(r => r.SaveAsync(It.IsAny<Session>()), Times.Never);
    }

    [Fact]
    public async Task Validate_WithExistingKey_ShouldReturnSession()
    {
        var session = new Session(SessionType.User, "40817123");
        _sessionRepository.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        SessionService service = CreateService();

        Session? result = await service.ValidateAsync(session.Key);

        result.Should().Be(session);
    }

    [Fact]
    public async Task Validate_WithUnknownKey_ShouldReturnNull()
    {
        _sessionRepository.Setup(r => r.GetByKeyAsync(It.IsAny<Guid>())).ReturnsAsync((Session?)null);

        SessionService service = CreateService();

        Session? result = await service.ValidateAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    private SessionService CreateService()
    {
        return new SessionService(_sessionRepository.Object, _accountRepository.Object, _adminPasswordValidator.Object);
    }
}
