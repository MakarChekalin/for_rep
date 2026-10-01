using Atm.Domain;
using Atm.Infrastructure;
using FluentAssertions;

namespace Atm.IntegrationTests.Repositories;

public class PostgresSessionRepositoryTests : RepositoryTestBase
{
    public PostgresSessionRepositoryTests(AtmDatabaseFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SaveAsync_ThenGetByKeyAsync_ShouldReturnUserSessionWithAccountNumber()
    {
        var repository = new PostgresSessionRepository(Fixture.ConnectionProvider);
        var session = new Session(SessionType.User, "40817123");

        await repository.SaveAsync(session);

        Session? loaded = await repository.GetByKeyAsync(session.Key);

        Assert.NotNull(loaded);
        loaded.Key.Should().Be(session.Key);
        loaded.Type.Should().Be(SessionType.User);
        loaded.AccountNumber.Should().Be("40817123");
    }

    [Fact]
    public async Task SaveAsync_ThenGetByKeyAsync_ShouldReturnAdminSessionWithNullAccountNumber()
    {
        var repository = new PostgresSessionRepository(Fixture.ConnectionProvider);
        var session = new Session(SessionType.Admin);

        await repository.SaveAsync(session);

        Session? loaded = await repository.GetByKeyAsync(session.Key);

        Assert.NotNull(loaded);
        loaded.Type.Should().Be(SessionType.Admin);
        loaded.AccountNumber.Should().BeNull();
    }

    [Fact]
    public async Task GetByKeyAsync_WithUnknownKey_ShouldReturnNull()
    {
        var repository = new PostgresSessionRepository(Fixture.ConnectionProvider);

        Session? loaded = await repository.GetByKeyAsync(Guid.NewGuid());

        loaded.Should().BeNull();
    }
}
