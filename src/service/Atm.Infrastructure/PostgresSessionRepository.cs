using Atm.Domain;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresSessionRepository : ISessionRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public PostgresSessionRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Session?> GetByKeyAsync(Guid key)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            "SELECT key, type, account_number FROM sessions WHERE key = @key",
            connection);
        command.Parameters.AddWithValue("key", key);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        SessionType type = Enum.Parse<SessionType>(reader.GetString(reader.GetOrdinal("type")));
        int accountNumberOrdinal = reader.GetOrdinal("account_number");
        string? accountNumber = reader.IsDBNull(accountNumberOrdinal) ? null : reader.GetString(accountNumberOrdinal);

        return new Session(type, accountNumber, reader.GetGuid(reader.GetOrdinal("key")));
    }

    public async Task SaveAsync(Session session)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync();

        await using var command = new NpgsqlCommand(
            @"
            INSERT INTO sessions (key, type, account_number)
            VALUES (@key, @type, @accountNumber)",
            connection);

        command.Parameters.AddWithValue("key", session.Key);
        command.Parameters.AddWithValue("type", session.Type.ToString());
        command.Parameters.AddWithValue("accountNumber", (object?)session.AccountNumber ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }
}