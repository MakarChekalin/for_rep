using Atm.Domain;
using Atm.Infrastructure;
using FluentAssertions;

namespace Atm.IntegrationTests.Repositories;

public class PostgresInvoiceRepositoryTests : RepositoryTestBase
{
    public PostgresInvoiceRepositoryTests(AtmDatabaseFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SaveAsync_ThenGetByIdAsync_ShouldReturnInvoice()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);
        var invoice = new Invoice("payer-1", "payee-1", 123.45m);

        await repository.SaveAsync(invoice);

        Invoice? loaded = await repository.GetByIdAsync(invoice.Id);

        Assert.NotNull(loaded);
        loaded.Id.Should().Be(invoice.Id);
        loaded.PayerAccountNumber.Should().Be("payer-1");
        loaded.PayeeAccountNumber.Should().Be("payee-1");
        loaded.Amount.Should().Be(123.45m);
        loaded.Status.Should().Be(InvoiceStatus.Created);
    }

    [Fact]
    public async Task GetByIdAsync_WithUnknownId_ShouldReturnNull()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);

        Invoice? loaded = await repository.GetByIdAsync(Guid.NewGuid());

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task SaveAsync_WhenInvoiceExists_ShouldUpdateStatusOnly()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);
        var invoice = new Invoice("payer-1", "payee-1", 100);
        await repository.SaveAsync(invoice);

        invoice.Pay();
        await repository.SaveAsync(invoice);

        Invoice? loaded = await repository.GetByIdAsync(invoice.Id);

        Assert.NotNull(loaded);
        loaded.Status.Should().Be(InvoiceStatus.Paid);
        loaded.Amount.Should().Be(100);
    }

    [Fact]
    public async Task GetOutgoingAsync_ShouldReturnInvoicesIssuedByAccount()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);
        await repository.SaveAsync(new Invoice("payer-1", "payee-1", 10));
        await repository.SaveAsync(new Invoice("payer-2", "payee-1", 20));
        await repository.SaveAsync(new Invoice("payer-1", "other-payee", 30));

        List<Invoice> result = await CollectAsync(repository.GetOutgoingAsync("payee-1", null, null, null, 10));

        result.Should().HaveCount(2);
        result.Should().OnlyContain(i => i.PayeeAccountNumber == "payee-1");
    }

    [Fact]
    public async Task GetOutgoingAsync_FilteredByPayerAndStatus_ShouldReturnOnlyMatching()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);

        var matching = new Invoice("payer-1", "payee-1", 10);
        var wrongPayer = new Invoice("payer-2", "payee-1", 20);
        var wrongStatus = new Invoice("payer-1", "payee-1", 30);
        wrongStatus.Cancel();

        await repository.SaveAsync(matching);
        await repository.SaveAsync(wrongPayer);
        await repository.SaveAsync(wrongStatus);

        List<Invoice> result = await CollectAsync(
            repository.GetOutgoingAsync("payee-1", "payer-1", InvoiceStatus.Created, null, 10));

        result.Should().ContainSingle();
        result[0].Id.Should().Be(matching.Id);
    }

    [Fact]
    public async Task GetOutgoingAsync_WithPagination_ShouldRespectCursorAndOrder()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);

        var first = new Invoice("payer-1", "payee-1", 10, createdAt: DateTime.UtcNow.AddMinutes(-3));
        var second = new Invoice("payer-1", "payee-1", 20, createdAt: DateTime.UtcNow.AddMinutes(-2));
        var third = new Invoice("payer-1", "payee-1", 30, createdAt: DateTime.UtcNow.AddMinutes(-1));

        await repository.SaveAsync(first);
        await repository.SaveAsync(second);
        await repository.SaveAsync(third);

        List<Invoice> firstPage = await CollectAsync(repository.GetOutgoingAsync("payee-1", null, null, null, 2));
        firstPage.Select(i => i.Id).Should().Equal(first.Id, second.Id);

        List<Invoice> secondPage = await CollectAsync(repository.GetOutgoingAsync("payee-1", null, null, firstPage[^1].Id, 2));
        secondPage.Select(i => i.Id).Should().Equal(third.Id);
    }

    [Fact]
    public async Task GetIncomingAsync_ShouldReturnInvoicesPayableByAccount()
    {
        var repository = new PostgresInvoiceRepository(Fixture.ConnectionProvider);
        await repository.SaveAsync(new Invoice("payer-1", "payee-1", 10));
        await repository.SaveAsync(new Invoice("payer-1", "payee-2", 20));

        List<Invoice> result = await CollectAsync(repository.GetIncomingAsync("payer-1", null, null, null, 10));

        result.Should().HaveCount(2);
        result.Should().OnlyContain(i => i.PayerAccountNumber == "payer-1");
    }

    private static async Task<List<Invoice>> CollectAsync(IAsyncEnumerable<Invoice> source)
    {
        var result = new List<Invoice>();

        await foreach (Invoice invoice in source)
            result.Add(invoice);

        return result;
    }
}
