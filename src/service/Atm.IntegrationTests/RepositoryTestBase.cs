namespace Atm.IntegrationTests;

[Collection(DatabaseCollection.Name)]
public abstract class RepositoryTestBase : IAsyncLifetime
{
    protected RepositoryTestBase(AtmDatabaseFixture fixture)
    {
        Fixture = fixture;
    }

    protected AtmDatabaseFixture Fixture { get; }

    public Task InitializeAsync() => Fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
