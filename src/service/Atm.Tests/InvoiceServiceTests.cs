using Atm.Application;
using Atm.Application.Results;
using Atm.Domain;
using Moq;

namespace Atm.Tests;

public class InvoiceServiceTests
{
    [Fact]
    public async Task CreateInvoice_WithExistingPayer_ShouldSucceed()
    {
        // Arrange
        var session = new Session(SessionType.User, "payee-1");

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.ExistsAsync("payer-1")).ReturnsAsync(true);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            accountRepoMock.Object,
            sessionRepoMock.Object,
            Mock.Of<IOperationRepository>(),
            TransactionProviderMock.Create());

        // Act
        CreateInvoiceResult result = await service.CreateInvoiceAsync(session.Key, "payer-1", 150);

        // Assert
        Assert.Equal(CreateInvoiceStatus.Success, result.Status);
        Assert.NotNull(result.InvoiceId);
        invoiceRepoMock.Verify(r => r.SaveAsync(It.IsAny<Invoice>()), Times.Once);
    }

    [Fact]
    public async Task CreateInvoice_WithUnknownPayer_ShouldReturnPayerNotFound()
    {
        // Arrange
        var session = new Session(SessionType.User, "payee-1");

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.ExistsAsync("payer-1")).ReturnsAsync(false);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            accountRepoMock.Object,
            sessionRepoMock.Object,
            Mock.Of<IOperationRepository>(),
            TransactionProviderMock.Create());

        // Act
        CreateInvoiceResult result = await service.CreateInvoiceAsync(session.Key, "payer-1", 150);

        // Assert
        Assert.Equal(CreateInvoiceStatus.PayerNotFound, result.Status);
        invoiceRepoMock.Verify(r => r.SaveAsync(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task PayInvoice_WithSufficientBalance_ShouldUpdateBothAccountsAndRecordTwoOperations()
    {
        // Arrange
        var payer = new Account("payer-1", "1111", 1000);
        var payee = new Account("payee-1", "2222", 0);
        var session = new Session(SessionType.User, "payer-1");
        var invoice = new Invoice("payer-1", "payee-1", 150);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();
        invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.GetByNumberAsync("payer-1")).ReturnsAsync(payer);
        accountRepoMock.Setup(r => r.GetByNumberAsync("payee-1")).ReturnsAsync(payee);

        var operationRepoMock = new Mock<IOperationRepository>();

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            accountRepoMock.Object,
            sessionRepoMock.Object,
            operationRepoMock.Object,
            TransactionProviderMock.Create());

        // Act
        PayInvoiceResult result = await service.PayInvoiceAsync(session.Key, invoice.Id);

        // Assert
        Assert.Equal(PayInvoiceResult.Success, result);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(850, payer.Balance);
        Assert.Equal(150, payee.Balance);
        operationRepoMock.Verify(r => r.SaveAsync(It.Is<Operation>(o => o.AccountNumber == "payer-1" && o.Type == OperationType.Withdraw)), Times.Once);
        operationRepoMock.Verify(r => r.SaveAsync(It.Is<Operation>(o => o.AccountNumber == "payee-1" && o.Type == OperationType.Deposit)), Times.Once);
    }

    [Fact]
    public async Task PayInvoice_WhenNotThePayer_ShouldReturnUnauthorized()
    {
        // Arrange
        var session = new Session(SessionType.User, "someone-else");
        var invoice = new Invoice("payer-1", "payee-1", 150);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();
        invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            Mock.Of<IAccountRepository>(),
            sessionRepoMock.Object,
            Mock.Of<IOperationRepository>(),
            TransactionProviderMock.Create());

        // Act
        PayInvoiceResult result = await service.PayInvoiceAsync(session.Key, invoice.Id);

        // Assert
        Assert.Equal(PayInvoiceResult.Unauthorized, result);
        Assert.Equal(InvoiceStatus.Created, invoice.Status);
    }

    [Fact]
    public async Task PayInvoice_WhenAlreadyPaid_ShouldReturnAlreadyProcessed()
    {
        // Arrange
        var payer = new Account("payer-1", "1111", 1000);
        var payee = new Account("payee-1", "2222", 0);
        var session = new Session(SessionType.User, "payer-1");
        var invoice = new Invoice("payer-1", "payee-1", 150);
        invoice.Pay();

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();
        invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.GetByNumberAsync("payer-1")).ReturnsAsync(payer);
        accountRepoMock.Setup(r => r.GetByNumberAsync("payee-1")).ReturnsAsync(payee);

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            accountRepoMock.Object,
            sessionRepoMock.Object,
            Mock.Of<IOperationRepository>(),
            TransactionProviderMock.Create());

        // Act
        PayInvoiceResult result = await service.PayInvoiceAsync(session.Key, invoice.Id);

        // Assert
        Assert.Equal(PayInvoiceResult.AlreadyProcessed, result);
        Assert.Equal(1000, payer.Balance);
    }

    [Fact]
    public async Task PayInvoice_WithInsufficientFunds_ShouldReturnInsufficientFunds()
    {
        // Arrange
        var payer = new Account("payer-1", "1111", 50);
        var payee = new Account("payee-1", "2222", 0);
        var session = new Session(SessionType.User, "payer-1");
        var invoice = new Invoice("payer-1", "payee-1", 150);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();
        invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        var accountRepoMock = new Mock<IAccountRepository>();
        accountRepoMock.Setup(r => r.GetByNumberAsync("payer-1")).ReturnsAsync(payer);
        accountRepoMock.Setup(r => r.GetByNumberAsync("payee-1")).ReturnsAsync(payee);

        var operationRepoMock = new Mock<IOperationRepository>();

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            accountRepoMock.Object,
            sessionRepoMock.Object,
            operationRepoMock.Object,
            TransactionProviderMock.Create());

        // Act
        PayInvoiceResult result = await service.PayInvoiceAsync(session.Key, invoice.Id);

        // Assert
        Assert.Equal(PayInvoiceResult.InsufficientFunds, result);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        operationRepoMock.Verify(r => r.SaveAsync(It.IsAny<Operation>()), Times.Never);
        invoiceRepoMock.Verify(r => r.SaveAsync(It.IsAny<Invoice>()), Times.Never);
    }

    [Fact]
    public async Task CancelInvoice_ByIssuer_ShouldSucceed()
    {
        // Arrange
        var session = new Session(SessionType.User, "payee-1");
        var invoice = new Invoice("payer-1", "payee-1", 150);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();
        invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            Mock.Of<IAccountRepository>(),
            sessionRepoMock.Object,
            Mock.Of<IOperationRepository>(),
            TransactionProviderMock.Create());

        // Act
        CancelInvoiceResult result = await service.CancelInvoiceAsync(session.Key, invoice.Id);

        // Assert
        Assert.Equal(CancelInvoiceResult.Success, result);
        Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        invoiceRepoMock.Verify(r => r.SaveAsync(invoice), Times.Once);
    }

    [Fact]
    public async Task CancelInvoice_ByPayer_ShouldReturnUnauthorized()
    {
        // Arrange
        var session = new Session(SessionType.User, "payer-1");
        var invoice = new Invoice("payer-1", "payee-1", 150);

        var sessionRepoMock = new Mock<ISessionRepository>();
        sessionRepoMock.Setup(r => r.GetByKeyAsync(session.Key)).ReturnsAsync(session);

        var invoiceRepoMock = new Mock<IInvoiceRepository>();
        invoiceRepoMock.Setup(r => r.GetByIdAsync(invoice.Id)).ReturnsAsync(invoice);

        var service = new InvoiceService(
            invoiceRepoMock.Object,
            Mock.Of<IAccountRepository>(),
            sessionRepoMock.Object,
            Mock.Of<IOperationRepository>(),
            TransactionProviderMock.Create());

        // Act
        CancelInvoiceResult result = await service.CancelInvoiceAsync(session.Key, invoice.Id);

        // Assert
        Assert.Equal(CancelInvoiceResult.Unauthorized, result);
        Assert.Equal(InvoiceStatus.Created, invoice.Status);
    }
}
