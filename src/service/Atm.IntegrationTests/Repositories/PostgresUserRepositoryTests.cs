using Atm.Domain;
using Atm.Infrastructure;
using FluentAssertions;

namespace Atm.IntegrationTests.Repositories;

public class PostgresUserRepositoryTests : RepositoryTestBase
{
    public PostgresUserRepositoryTests(AtmDatabaseFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SaveAsync_ThenExistsAsync_ShouldReturnTrue()
    {
        var repository = new PostgresUserRepository(Fixture.ConnectionProvider);
        var user = new User("user-1", DateTime.UtcNow);

        await repository.SaveAsync(user);

        bool exists = await repository.ExistsAsync("user-1");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithUnknownId_ShouldReturnFalse()
    {
        var repository = new PostgresUserRepository(Fixture.ConnectionProvider);

        bool exists = await repository.ExistsAsync("ghost");

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task SaveAsync_WhenAlreadyExists_ShouldBeIdempotent()
    {
        var repository = new PostgresUserRepository(Fixture.ConnectionProvider);
        var user = new User("user-1", DateTime.UtcNow);

        await repository.SaveAsync(user);
        await repository.SaveAsync(user);

        bool exists = await repository.ExistsAsync("user-1");

        exists.Should().BeTrue();
    }
}
