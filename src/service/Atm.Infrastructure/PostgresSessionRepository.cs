using Atm.Domain;
using Itmo.Dev.Platform.Persistence.Abstractions.Commands;
using Itmo.Dev.Platform.Persistence.Abstractions.Connections;
using System.Data.Common;

namespace Atm.Infrastructure;

public class PostgresSessionRepository : ISessionRepository
{
    private readonly IPersistenceConnectionProvider _connectionProvider;

    public PostgresSessionRepository(IPersistenceConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<Session?> GetByKeyAsync(Guid key)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("select key, type, account_number from sessions where key = @key")
            .AddParameter("key", key);

        await using DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);

        if (!await reader.ReadAsync())
            return null;

        SessionType type = Enum.Parse<SessionType>(reader.GetString(reader.GetOrdinal("type")));
        int accountNumberOrdinal = reader.GetOrdinal("account_number");
        string? accountNumber = reader.IsDBNull(accountNumberOrdinal) ? null : reader.GetString(accountNumberOrdinal);

        return new Session(type, accountNumber, reader.GetGuid(reader.GetOrdinal("key")));
    }

    public async Task SaveAsync(Session session)
    {
        await using IPersistenceConnection connection = await _connectionProvider.GetConnectionAsync(CancellationToken.None);

        await using IPersistenceCommand command = connection
            .CreateCommand("""
                insert into sessions (key, type, account_number)
                values (@key, @type, @accountNumber)
                """)
            .AddParameter("key", session.Key)
            .AddParameter<string>("type", session.Type.ToString())
            .AddParameter<string?>("accountNumber", session.AccountNumber);

        await command.ExecuteNonQueryAsync(CancellationToken.None);
    }
}
