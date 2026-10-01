using Atm.Domain;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data.Common;

namespace Atm.Infrastructure;

public class PostgresUserRepository : IUserRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public PostgresUserRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<bool> ExistsAsync(string id)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("select 1 from users where id = @id")
            .AddParameter<string>("id", id);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        return await reader.ReadAsync();
    }

    public async Task<bool> ExistsByExternalIdAsync(long externalId)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("select 1 from users where external_id = @externalId")
            .AddParameter("externalId", externalId);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        return await reader.ReadAsync();
    }

    public async Task<long?> GetExternalIdAsync(string id)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("select external_id from users where id = @id")
            .AddParameter<string>("id", id);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        if (!await reader.ReadAsync())
            return null;

        return reader.GetInt64(0);
    }

    public async Task SaveAsync(User user)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("""
                insert into users (id, created_at)
                values (@id, @createdAt)
                on conflict (id) do nothing
                """)
            .AddParameter<string>("id", user.Id)
            .AddParameter("createdAt", user.CreatedAt);

        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }
}
