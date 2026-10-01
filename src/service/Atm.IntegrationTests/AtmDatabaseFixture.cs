using Atm.Infrastructure;
using FluentMigrator.Runner;
using Itmo.Dev.Platform.Common.Serialization;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using Itmo.Dev.Platform.Testing.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Atm.IntegrationTests;

public sealed class AtmDatabaseFixture : DatabaseFixture
{
    public AtmDatabaseFixture()
        : base(createRespawnerOnInitialization: true)
    {
    }

    public IPersistenceConnectionProvider ConnectionProvider => Provider.GetRequiredService<IPersistenceConnectionProvider>();

    public IPlatformSerializer Serializer => Provider.GetRequiredService<IPlatformSerializer>();

    public string ConnectionString => Container.GetConnectionString();

    protected override void ConfigureServices(IServiceCollection collection)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(Container.GetConnectionString());

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Infrastructure:Persistence:Postgres:Host"] = connectionString.Host,
                ["Infrastructure:Persistence:Postgres:Port"] = connectionString.Port.ToString(),
                ["Infrastructure:Persistence:Postgres:Database"] = connectionString.Database,
                ["Infrastructure:Persistence:Postgres:Username"] = connectionString.Username,
                ["Infrastructure:Persistence:Postgres:Password"] = connectionString.Password,
                ["AdminPassword"] = "integration-tests-admin-password",
            })
            .Build();

        collection.AddSingleton(configuration);
        collection.AddPersistence();
    }

    protected override async ValueTask UseProviderAsync(IServiceProvider provider)
    {
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
