using Atm.Domain;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresSessionRepository : ISessionRepository
{
    private readonly string _connectionString;

    public PostgresSessionRepository(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<Session?> GetByKeyAsync(Guid key)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT key, type, account_number FROM sessions WHERE key = @key",
            connection);
        command.Parameters.AddWithValue("key", key);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        var type = Enum.Parse<SessionType>(reader.GetString(reader.GetOrdinal("type")));
        int accountNumberOrdinal = reader.GetOrdinal("account_number");
        string? accountNumber = reader.IsDBNull(accountNumberOrdinal) ? null : reader.GetString(accountNumberOrdinal);

        return new Session(type, accountNumber, reader.GetGuid(reader.GetOrdinal("key")));
    }

    public async Task SaveAsync(Session session)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

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