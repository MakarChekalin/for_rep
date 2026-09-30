using Atm.Application;
using Atm.Application.Results;
using Atm.Domain;
using Moq;

namespace Atm.Tests;

public class AccountServiceTests
{
    [Fact]
    public async Task Withdraw_WithSufficientBalance_ShouldUpdateBalance()
    {
        // Arrange
        var account = new Account("40817123", "1234", 1000);
        var session = new Session(SessionType.User, "40817123");

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.GetByNumberAsync("40817123")).ReturnsAsync(account);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var operationRepoMock = new Mock<IOperationRepository>();

        var service = new AccountService(accountRepoMock.Object, sessionRepoMock.Object, operationRepoMock.Object);

        // Act
        WithdrawResult result = await service.WithdrawAsync(session.Key, 300);

        // Assert
        Assert.Equal(WithdrawResult.Success, result);
        Assert.Equal(700, account.Balance);
        accountRepoMock.Verify(r => r.SaveAsync(account), Times.Once);
    }

    [Fact]
    public async Task Withdraw_WithInsufficientBalance_ShouldReturnError()
    {
        // Arrange
        var account = new Account("40817123", "1234", 100);
        var session = new Session(SessionType.User, "40817123");

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.GetByNumberAsync("40817123")).ReturnsAsync(account);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var operationRepoMock = new Mock<IOperationRepository>();

        var service = new AccountService(accountRepoMock.Object, sessionRepoMock.Object, operationRepoMock.Object);

        // Act
        WithdrawResult result = await service.WithdrawAsync(session.Key, 300);

        // Assert
        Assert.Equal(WithdrawResult.InsufficientFunds, result);
        Assert.Equal(100, account.Balance);
        accountRepoMock.Verify(r => r.SaveAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task Deposit_ShouldUpdateBalance()
    {
        // Arrange
        var account = new Account("40817123", "1234", 500);
        var session = new Session(SessionType.User, "40817123");

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.GetByNumberAsync("40817123")).ReturnsAsync(account);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var operationRepoMock = new Mock<IOperationRepository>();

        var service = new AccountService(accountRepoMock.Object, sessionRepoMock.Object, operationRepoMock.Object);

        // Act
        DepositResult result = await service.DepositAsync(session.Key, 250);

        // Assert
        Assert.Equal(DepositResult.Success, result);
        Assert.Equal(750, account.Balance);
        accountRepoMock.Verify(r => r.SaveAsync(account), Times.Once);
    }
}