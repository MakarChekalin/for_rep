using Atm.Domain;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Atm.Infrastructure;

public class PostgresOperationRepository : IOperationRepository
{
    private readonly string _connectionString;

    public PostgresOperationRepository(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task SaveAsync(Operation operation)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            @"
            INSERT INTO operations (account_number, type, amount, timestamp)
            VALUES (@accountNumber, @type, @amount, @timestamp)",
            connection);

        command.Parameters.AddWithValue("accountNumber", operation.AccountNumber);
        command.Parameters.AddWithValue("type", operation.Type.ToString());
        command.Parameters.AddWithValue("amount", operation.Amount);
        command.Parameters.AddWithValue("timestamp", operation.Timestamp);

        await command.ExecuteNonQueryAsync();
    }

    // IAsyncEnumerable: операции отдаются по одной по мере чтения из БД, без накопления всего списка в памяти
    public async IAsyncEnumerable<Operation> GetByAccountNumberAsync(string accountNumber)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            "SELECT account_number, type, amount, timestamp FROM operations WHERE account_number = @accountNumber ORDER BY timestamp",
            connection); // сортировка по времени
        command.Parameters.AddWithValue("accountNumber", accountNumber);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var type = Enum.Parse<OperationType>(reader.GetString(reader.GetOrdinal("type")));

            yield return new Operation(
                reader.GetString(reader.GetOrdinal("account_number")),
                type,
                reader.GetDecimal(reader.GetOrdinal("amount")),
                reader.GetDateTime(reader.GetOrdinal("timestamp")));
        }
    }
}